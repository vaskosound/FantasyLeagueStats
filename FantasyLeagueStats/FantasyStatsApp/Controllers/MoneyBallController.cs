using FantasyStatsApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FantasyStatsApp.Controllers
{
    [Authorize]
    public class MoneyBallController : BaseController
    {      
        public ActionResult TeamByPoints()
        {
            return View();
        }

        public ActionResult GetTeamByPoints(SubmitTeamPrice price)
        {
            var players = this.Data.Players.All().OrderByDescending(p => p.Points)
                .Take(100).Select(x => new PlayerValuableModel() 
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Points,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials
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

        public ActionResult GetTeamByPointsPerGame(SubmitTeamPrice price)
        {
            var players = this.Data.Players.All().OrderByDescending(p => p.PointsPerGame)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.PointsPerGame,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials
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

        public ActionResult GetTeamByPointsPerPrice(SubmitTeamPrice price)
        {
            var players = this.Data.Players.All().OrderByDescending(p => p.Points / p.Price)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Points / x.Price,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials
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

        public ActionResult GetTeamByPPPPerGame(SubmitTeamPrice price)
        {
            var players = this.Data.Players.All().OrderByDescending(x => x.PointsPerGame / x.Price)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.PointsPerGame / x.Price,
                    Weight = x.Price,
                    Position = x.Position,
                    Team = x.Team.Initials
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();

            return PartialView("_ValuableTeam", bestTeam);
        }
    }
}