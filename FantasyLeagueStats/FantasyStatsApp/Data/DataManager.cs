using FantasyStats.Data;
using FantasyStats.Model;
using FantasyStatsApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FantasyStatsApp.Data
{
    public class DataManager
    {

        private const string Мan_United = "Man Utd";
        private const string Мan_City = "Man City";
        private static DateTime startDate = new DateTime(2013, 7, 15);

        public void UpdateBasicData(List<string> stats)
        {
            for (int i = 0; i < stats.Count; i += 8)
            {
                var playerModel = new PlayerModel()
                {
                    Name = stats[i],
                    Selected = double.Parse(stats[i + 3].TrimEnd('%')),
                    Points = int.Parse(stats[i + 6]),
                    Team = stats[i + 1],
                    MinutesPlayed = int.Parse(stats[i + 7])
                };
                playerModel.SetPosition(stats[i + 2]);
                playerModel.SetPrice(stats[i + 4]);

                AddOrUpdatePlayer(playerModel);
            }
        }

        public void UpdatePointsPerGameData(
            List<string> stats)
        {
            var context = new FantasyStatsDbContext();
            for (int i = 0; i < stats.Count; i += 8)
            {
                var playerModel = new PlayerModel()
                {
                    Name = stats[i],
                    PointsPerGame = decimal.Parse(stats[i + 7])
                };

                var playerExists = context.Players.FirstOrDefault(x => x.Name == playerModel.Name);
                if (playerExists != null)
                {
                    playerExists.PointsPerGame = playerModel.PointsPerGame;
                }
            }

            context.SaveChanges();
        }

        public void UpdateStandings(List<string> statsStandings)
        {
            var context = new FantasyStatsDbContext();
            for (int i = 14; i < statsStandings.Count; i += 12)
            {
                string teamName = statsStandings[i + 3];
                var team = context.Teams.FirstOrDefault(t => t.Name == teamName);
                team.Position = int.Parse(statsStandings[i]);
                team.GamesPlayed = int.Parse(statsStandings[i + 4]);
                team.Wins = int.Parse(statsStandings[i + 5]);
                team.Draws = int.Parse(statsStandings[i + 6]);
                team.Losses = int.Parse(statsStandings[i + 7]);
                team.GoalsFor = int.Parse(statsStandings[i + 8]);
                team.GoalsAgainst = int.Parse(statsStandings[i + 9]);
                team.Points = int.Parse(statsStandings[i + 11]);
            }

            context.SaveChanges();
        }

        public void UpdateFixtures(List<string> fixtures)
        {
            var context = new FantasyStatsDbContext();
            for (int i = 1; i < fixtures.Count; i += 4)
            {
                int index = fixtures[i].LastIndexOf(' ');
                string dateAsString = fixtures[i].Substring(0, index) + " " + DateTime.Now.Year;
                DateTime matchDate = DateTime.ParseExact(dateAsString, "dd MMM yyyy", CultureInfo.InvariantCulture);
                matchDate = matchDate < startDate ? matchDate.AddYears(1) : matchDate;
                string host = ConvertTeamName(fixtures[i + 1]);
                string visitor = ConvertTeamName(fixtures[i + 3]);

                var matchModel = new MatchViewModel()
                {
                    Host = host,
                    Visitor = visitor,
                    Gameweek = fixtures[0],
                    MatchDate = matchDate
                };
                if (fixtures[i + 2] != "v")
                {
                    string[] result = fixtures[i + 2].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    matchModel.HostScore = int.Parse(result[0]);
                    matchModel.VisitorScore = int.Parse(result[1]);
                }

                AddOrUpdateMatch(context, matchModel);
            }

            context.SaveChanges();
        }
               
        private void AddOrUpdatePlayer(PlayerModel playerModel)
        {
            var context = new FantasyStatsDbContext();

            var playerExists = context.Players.FirstOrDefault(x => x.Name == playerModel.Name);
            var team = context.Teams.FirstOrDefault(x => x.Initials == playerModel.Team);
            if (playerExists == null)
            {
                Player newPlayer = new Player()
                {
                    Name = playerModel.Name,
                    Position = playerModel.Position,
                    Selected = playerModel.Selected,
                    Price = playerModel.Price,
                    Points = playerModel.Points,
                    MinutesPlayed = playerModel.MinutesPlayed
                };
                team.Players.Add(newPlayer);
            }
            else
            {
                playerExists.TeamId = team.Id;
                playerExists.Selected = playerModel.Selected;
                playerExists.Points = playerModel.Points;
                playerExists.Price = playerModel.Price;
                playerExists.MinutesPlayed = playerModel.MinutesPlayed;
            }

            context.SaveChanges();
        }

        private void AddOrUpdateMatch(FantasyStatsDbContext context,
           MatchViewModel matchModel)
        {
            var matchExists = context.Matches
                .FirstOrDefault(m => (m.Host.Name.Contains(matchModel.Host)) &&
                    (m.Visitor.Name.Contains(matchModel.Visitor)));

            var hostTeam = context.Teams.FirstOrDefault(x => x.Name.Contains(matchModel.Host));
            var visitorTeam = context.Teams.FirstOrDefault(x => x.Name.Contains(matchModel.Visitor));
            var gameweekEntity = AddOrUpdateGameweek(context, matchModel);

            if (matchExists == null)
            {
                var newMatch = new Match()
                {
                    Gameweek = gameweekEntity,
                    Host = hostTeam,
                    Visitor = visitorTeam,
                    HostScore = matchModel.HostScore,
                    VistorScore = matchModel.VisitorScore,
                    MatchDate = matchModel.MatchDate
                };

                context.Matches.Add(newMatch);
            }
            else
            {
                matchExists.HostScore = matchModel.HostScore;
                matchExists.VistorScore = matchModel.VisitorScore;
            }
        }

        private Gameweek AddOrUpdateGameweek(FantasyStatsDbContext context,
            MatchViewModel matchModel)
        {
            var gameweek = context.Gameweeks.FirstOrDefault(g => g.Name == matchModel.Gameweek);

            int gamewekIndex = matchModel.Gameweek.LastIndexOf(' ');
            int gameweekNumber = int.Parse(matchModel.Gameweek.Substring(gamewekIndex + 1));
            var previousGameweek = context.Gameweeks.Find(gameweekNumber - 1);
            if (previousGameweek != null)
            {
                startDate = previousGameweek.EndDate;
            }

            if (gameweek == null)
            {
                var newGameweek = new Gameweek()
                {
                    Id = gameweekNumber,
                    Name = matchModel.Gameweek,
                    StartDate = startDate,
                    EndDate = matchModel.MatchDate.AddDays(1)
                };

                context.Gameweeks.Add(newGameweek);
                context.SaveChanges();
                gameweek = context.Gameweeks.Find(newGameweek.Id);
            }
            else
            {
                gameweek.StartDate = startDate;
                gameweek.EndDate = matchModel.MatchDate.AddDays(1);
            }

            return gameweek;
        }

        private string ConvertTeamName(string name)
        {
            if (name == Мan_United)
            {
                return "Manchester United";
            }

            if (name == Мan_City)
            {
                return "Manchester City";
            }

            return name;
        }
    }
}