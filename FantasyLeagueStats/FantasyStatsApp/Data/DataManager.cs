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
        private const string INJURED_ICON = "infowarn.png";
        private const string МAN_UNITED = "Man Utd";
        private const string МAN_CITY = "Man City";
        private const string TOTTENHAM = "Spurs";
        private const string QPR = "QPR";
        private static DateTime startSeason = new DateTime(2014, 7, 15);
        private static DateTime startDate;
        private IUowData data;

        public DataManager()
        {
            data = new UowData();
        }

        public void UpdateBasicData(List<string> stats)
        {
            List<Player> players = new List<Player>();
            DateTime currentDate = DateTime.Today;
            for (int i = 0; i < stats.Count; i += 9)
            {
                var playerModel = new PlayerModel()
                {
                    Name = stats[i + 1],
                    Selected = double.Parse(stats[i + 4].TrimEnd('%')),
                    Points = int.Parse(stats[i + 7]),
                    Team = stats[i + 2],
                    RoundScore = int.Parse(stats[i + 6]),
                    MinutesPlayed = int.Parse(stats[i + 8].Replace(",", "")),
                    IsInjured = stats[i].Contains(INJURED_ICON) ? true : false,   
                    UpadatedDate = DateTime.Today
                };


                playerModel.UpadatedDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                playerModel.SetPosition(stats[i + 3]);
                playerModel.SetPrice(stats[i + 5]);

                Player player = AddOrUpdatePlayer(playerModel);
                players.Add(player);
            }

            List<int> playerIds = players.Select(p => p.Id).ToList();

            data.Players.DeleteRange(p => !playerIds.Contains(p.Id));
            data.SaveChanges();
        }

        public void UpdatePointsPerGameData(
            List<string> stats)
        {
            for (int i = 0; i < stats.Count; i += 9)
            {
                var playerModel = new PlayerModel()
                {
                    Name = stats[i + 1],
                    Team = stats[i + 2],
                    PointsPerGame = decimal.Parse(stats[i + 8])
                };

                var playerExists = data.Players.All()
                    .FirstOrDefault(x => x.Name == playerModel.Name && x.Team.Initials == playerModel.Team);
                if (playerExists != null)
                {
                    playerExists.PointsPerGame = playerModel.PointsPerGame;
                }
            }

            data.SaveChanges();
        }

        public void UpdatePlayersForm(
            List<string> stats)
        {
            for (int i = 0; i < stats.Count; i += 9)
            {
                var playerModel = new PlayerModel()
                {
                    Name = stats[i + 1],
                    Team = stats[i + 2],
                    PlayerForm = decimal.Parse(stats[i + 8])
                };

                var playerExists = data.Players.All()
                    .FirstOrDefault(x => x.Name == playerModel.Name && x.Team.Initials == playerModel.Team);
                if (playerExists != null)
                {
                    playerExists.PlayerForm = playerModel.PlayerForm;
                }
            }

            data.SaveChanges();
        }

        public void UpdateStandings(List<string> statsStandings)
        {
            for (int i = 14; i < statsStandings.Count; i += 12)
            {
                string teamName = statsStandings[i + 3];
                var team = data.Teams.All().FirstOrDefault(t => t.Name == teamName);
                if (team == null)
                {
                    continue;
                }
                team.Position = int.Parse(statsStandings[i]);
                team.GamesPlayed = int.Parse(statsStandings[i + 4]);
                team.Wins = int.Parse(statsStandings[i + 5]);
                team.Draws = int.Parse(statsStandings[i + 6]);
                team.Losses = int.Parse(statsStandings[i + 7]);
                team.GoalsFor = int.Parse(statsStandings[i + 8]);
                team.GoalsAgainst = int.Parse(statsStandings[i + 9]);
                team.Points = int.Parse(statsStandings[i + 11]);
            }

            data.SaveChanges();
        }

        public void UpdateFixtures(List<string> fixtures)
        {
            startDate = startSeason;
            List<Match> gameweekMatches = new List<Match>();
            string gameweek = fixtures[0];
            for (int i = 1; i < fixtures.Count; i += 4)
            {
                string[] dateParts = fixtures[i].Split(' ');
                string dateAsString = string.Format("{0} {1} {2} {3}", dateParts[0], dateParts[1], startSeason.Year, dateParts[2]);
                DateTime matchDate = DateTime.ParseExact(dateAsString, "dd MMM yyyy HH:mm", CultureInfo.InvariantCulture);
                matchDate = matchDate < startDate ? matchDate.AddYears(1) : matchDate;
                string host = ConvertTeamName(fixtures[i + 1]);
                string visitor = ConvertTeamName(fixtures[i + 3]);

                var matchModel = new MatchViewModel()
                {
                    Host = host,
                    Visitor = visitor,
                    Gameweek = gameweek,
                    MatchDate = matchDate
                };
                if (fixtures[i + 2] != "v")
                {
                    string[] result = fixtures[i + 2].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    matchModel.HostScore = int.Parse(result[0]);
                    matchModel.VisitorScore = int.Parse(result[1]);
                }

                Match match = AddOrUpdateMatch(matchModel);
                if (match != null)
                {
                    this.data.SaveChanges();
                    gameweekMatches.Add(match);
                }
            }

            Gameweek currentGameweek = data.Gameweeks.All().FirstOrDefault(g => g.Name == gameweek);
            List<int> gameweekMatchesIds = gameweekMatches.Select(g => g.Id).ToList();
            data.Matches.DeleteRange(m => m.GameweekId == currentGameweek.Id && !gameweekMatchesIds.Contains(m.Id));

            data.SaveChanges();
        }

        public void UpdateDeadlines(List<string> deadlines)
        {
            startDate = startSeason;
            for (int i = 0; i < deadlines.Count; i += 2)
            {
                int gamewekIndex = deadlines[i].LastIndexOf(' ');
                int gameweekNumber = int.Parse(deadlines[i].Substring(gamewekIndex + 1));
                var gameweek = data.Gameweeks.GetById(gameweekNumber);
                if (gameweek != null)
                {
                    int index = deadlines[i + 1].LastIndexOf(' ');
                    string daeadlineAsString = deadlines[i + 1].Substring(0, index) + " " +
                        gameweek.StartDate.Year.ToString() + deadlines[i + 1].Substring(index);
                    DateTime deadline = DateTime.ParseExact(daeadlineAsString,
                        "d MMM yyyy HH:mm", CultureInfo.InvariantCulture);
                    gameweek.Deadline = deadline;
                    if (deadline < gameweek.StartDate)
                    {
                       gameweek.Deadline = deadline.AddYears(1);
                    }

                    var nextGameweek = data.Gameweeks.GetById(gameweekNumber + 1);
                    if (nextGameweek != null)
                    {
                        nextGameweek.StartDate = gameweek.Deadline.Value;
                    }
                }
            }

            data.SaveChanges();
        }

        private Player AddOrUpdatePlayer(PlayerModel playerModel)
        {
            DateTime currentSeason = DateTime.Now.Month >= 7 ? new DateTime(DateTime.Now.Year, 7, 1) :
                new DateTime(DateTime.Now.Year - 1, 7, 1);
            var playerExists = data.Players.All()
                .FirstOrDefault(x => x.Name == playerModel.Name && x.Team.Initials == playerModel.Team &&
                    x.UpadetedDate >= currentSeason);
            var team = data.Teams.All().FirstOrDefault(x => x.Initials == playerModel.Team);
            if (playerExists == null)
            {
                Player newPlayer = new Player()
                {
                    Name = playerModel.Name,
                    Position = playerModel.Position,
                    Selected = playerModel.Selected,
                    Price = playerModel.Price,
                    RoundScore = playerModel.RoundScore,
                    Points = playerModel.Points,
                    MinutesPlayed = playerModel.MinutesPlayed,
                    IsInjured = playerModel.IsInjured,
                    UpadetedDate = playerModel.UpadatedDate
                };
                team.Players.Add(newPlayer);
                playerExists = newPlayer;

            }
            else
            {
                playerExists.TeamId = team.Id;
                playerExists.Selected = playerModel.Selected;
                playerExists.Points = playerModel.Points;
                playerExists.Price = playerModel.Price;
                playerExists.RoundScore = playerModel.RoundScore;
                playerExists.MinutesPlayed = playerModel.MinutesPlayed;
                playerExists.IsInjured = playerModel.IsInjured;
                playerExists.UpadetedDate = playerModel.UpadatedDate;
            }

            data.SaveChanges();
            return playerExists;
        }

        private Match AddOrUpdateMatch(MatchViewModel matchModel)
        {
            var matchExists = data.Matches.All()
                .FirstOrDefault(m => (m.Host.Name.Contains(matchModel.Host)) &&
                    (m.Visitor.Name.Contains(matchModel.Visitor)));

            var hostTeam = data.Teams.All().FirstOrDefault(x => x.Name.Contains(matchModel.Host));
            var visitorTeam = data.Teams.All().FirstOrDefault(x => x.Name.Contains(matchModel.Visitor));
            if (hostTeam == null || visitorTeam == null)
            {
                return null;
            }

            var gameweekEntity = AddOrUpdateGameweek(matchModel);

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

                data.Matches.Add(newMatch);
                return newMatch;
            }
            else
            {
                matchExists.HostScore = matchModel.HostScore;
                matchExists.VistorScore = matchModel.VisitorScore;
                matchExists.MatchDate = matchModel.MatchDate;
                matchExists.Gameweek = gameweekEntity;

                return matchExists;
            }
        }

        private Gameweek AddOrUpdateGameweek(MatchViewModel matchModel)
        {
            var gameweek = data.Gameweeks.All().FirstOrDefault(g => g.Name == matchModel.Gameweek);

            int gamewekIndex = matchModel.Gameweek.LastIndexOf(' ');
            int gameweekNumber = int.Parse(matchModel.Gameweek.Substring(gamewekIndex + 1));
            var previousGameweek = data.Gameweeks.GetById(gameweekNumber - 1);
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

                data.Gameweeks.Add(newGameweek);
                data.SaveChanges();
                gameweek = data.Gameweeks.GetById(newGameweek.Id);
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
            if (name == МAN_UNITED)
            {
                return "Manchester United";
            }

            if (name == МAN_CITY)
            {
                return "Manchester City";
            }

            if (name == TOTTENHAM)
            {
                return "Tottenham Hotspur";
            }

            if (name == QPR)
            {
                return "Queens Park Rangers";
            }

            return name;
        }
    }
}