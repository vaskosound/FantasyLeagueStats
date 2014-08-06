using FantasyStatsApp.Data;
using FantasyStatsApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FantasyStatsApp.Controllers
{
    [Authorize]
    public class MoneyBallController : BaseController
    {
        private static DateTime currentSeason = DateTime.Now.Month >= 7 ? new DateTime(DateTime.Now.Year, 7, 1) :
                new DateTime(DateTime.Now.Year - 1, 7, 1);
        public ActionResult TeamByPoints()
        {
            return View();
        }

        public ActionResult GetTeamByPoints(SubmitTeamPriceModel price)
        {
            var players = this.Data.Players.All().Where(p => p.UpadetedDate >= currentSeason)
                .OrderByDescending(p => p.Points)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Points,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials,
                    ClubName = x.Team.Name,
                    IsInjured = x.IsInjured
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();

            return PartialView("_ValuableTeam", bestTeam);
        }

        public ActionResult TeamByPointsPerGame()
        {
            return View();
        }

        public ActionResult GetTeamByPointsPerGame(SubmitTeamPriceModel price)
        {
            var players = this.Data.Players.All().Where(p => p.UpadetedDate >= currentSeason)
                .OrderByDescending(p => p.PointsPerGame)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.PointsPerGame,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials,
                    ClubName = x.Team.Name,
                    IsInjured = x.IsInjured
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();

            return PartialView("_ValuableTeam", bestTeam);
        }

        public ActionResult TeamByPointsPerPrice()
        {
            return View();
        }

        public ActionResult GetTeamByPointsPerPrice(SubmitTeamPriceModel price)
        {
            var players = this.Data.Players.All().Where(p => p.UpadetedDate >= currentSeason)
                .OrderByDescending(p => p.Points / p.Price)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Points / x.Price,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials,
                    ClubName = x.Team.Name,
                    IsInjured = x.IsInjured
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();

            return PartialView("_ValuableTeam", bestTeam);
        }

        public ActionResult TeamByPPPPerGame()
        {
            return View();
        }

        public ActionResult GetTeamByPPPPerGame(SubmitTeamPriceModel price)
        {
            var players = this.Data.Players.All().Where(p => p.UpadetedDate >= currentSeason)
                .OrderByDescending(x => x.PointsPerGame / x.Price)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.PointsPerGame / x.Price,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials,
                    ClubName = x.Team.Name,
                    IsInjured = x.IsInjured
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();

            return PartialView("_ValuableTeam", bestTeam);
        }

        //private void GetOpponentsTeamsRatio()
        //{
        //    var players = this.Data.Players.All().Select(PlayerBasicModel.FromPlayersStats).ToList();
        //    DateTime currentDate = DateTime.Now;
        //    foreach (var player in players)
        //    {
        //        var playerMatches = this.Data.Matches.All().Where(m => (m.MatchDate > currentDate) &&
        //            (m.Visitor.Initials == player.Team || m.Host.Initials == player.Team)).Take(3);

        //        var teamAsVisitor = playerMatches.Where(v => v.Visitor.Initials == player.Team)
        //            .Select(HostTeamModel.FromMatch).Sum(x => (double)(x.GoalsFor * x.Points) / x.Position);
        //        var teamAsHost = playerMatches.Where(v => v.Host.Initials == player.Team)
        //            .Select(VisitorTeamModel.FromMatch).ToList();
        //    }       
        //}
    }
}