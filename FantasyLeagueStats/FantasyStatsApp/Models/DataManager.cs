using FantasyStats.Data;
using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class DataManager
    {

        private const string manUnited = "Man Utd";
        private const string manCity = "Man City";
        public static void UpdateBasicData(
            List<string> stats)
        {
            var context = new ApplicationDbContext();
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
        }

        public static void UpdatePointsPerGameData(
            List<string> stats)
        {
            var context = new ApplicationDbContext();
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

        public static void UpdateStandings(List<string> statsStandings)
        {
            var context = new ApplicationDbContext();
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

        public static void UpdateFixtures(List<string> fixtures)
        {
            var context = new ApplicationDbContext();
            for (int i = 1; i < fixtures.Count; i += 4)
            {
                int index = fixtures[i].LastIndexOf(' ');
                string dateAsString = fixtures[i].Substring(0, index) + " 2013";
                string host = ConvertTeamName(fixtures[i + 1]);
                string visitor = ConvertTeamName(fixtures[i + 3]);
                var matchModel = new MatchViewModel()
                {
                    Host = host,
                    Visitor = visitor,
                    Gameweek = fixtures[0],
                    MatchDate = DateTime.ParseExact(dateAsString, "dd MMM yyyy", CultureInfo.InvariantCulture)
                };
                if (fixtures[i + 2] != "v")
                {
                    string[] result = fixtures[i + 2].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    matchModel.HostScore = int.Parse(result[0]);
                    matchModel.VisitorScore = int.Parse(result[1]);
                }
                
                var matchExists = context.Matches
                    .FirstOrDefault(m => (m.Host.Name.Contains(matchModel.Host)) &&
                        (m.Visitor.Name.Contains(matchModel.Visitor)));
                
                var hostTeam = context.Teams.FirstOrDefault(x => x.Name.Contains(matchModel.Host));
                var visitorTeam = context.Teams.FirstOrDefault(x => x.Name.Contains(matchModel.Visitor));
                var gameweekEntity = context.Gameweeks.FirstOrDefault(g => g.Name == matchModel.Gameweek);

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

                context.SaveChanges();
            }

            context.SaveChanges();
        }

        private static string ConvertTeamName(string name)
        {
            if (name == manUnited)
            {
                return "Manchester United";
            }

            if (name == manCity)
            {
                return "Manchester City";
            }

            return name;
        }

        internal static void UpdateGameweeks(List<string> gameweeks)
        {
            var context = new ApplicationDbContext();
            DateTime startDate = new DateTime(2013, 7, 15);
            for (int i = 0; i < gameweeks.Count; i += 2)
            {
                int index = gameweeks[i].LastIndexOf(' ');
                int gameweekNumber = int.Parse(gameweeks[i].Substring(index + 1));
                int dateIndex = gameweeks[i + 1].LastIndexOf(' ');
                string dateAsString = gameweeks[i + 1].Substring(0, dateIndex) + " 2013";
                var gameweek = new Gameweek()
                {
                    Id = gameweekNumber,
                    Name = "Gameweek " + gameweekNumber,
                    StartDate = startDate,
                    EndDate = DateTime.ParseExact(dateAsString, "d MMM yyyy", CultureInfo.InvariantCulture)
                };
                context.Gameweeks.Add(gameweek);
                startDate = gameweek.EndDate;
            }

            context.SaveChanges();
        }
    }
}