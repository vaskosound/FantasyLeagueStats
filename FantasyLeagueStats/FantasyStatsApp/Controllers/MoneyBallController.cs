using FantasyStats.Data;
using FantasyStats.Model;
using FantasyStatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FantasyStatsApp.Controllers
{
    [Authorize]
    public class MoneyBallController : Controller
    {
        public ApplicationDbContext Data { get; set; }

        public MoneyBallController()
        {
            this.Data = new ApplicationDbContext();
        }
        public ActionResult TeamByPoints()
        {
            return View();
        }

        public ActionResult GetTeamByPoints(SubmitTeamPrice price)
        {
            var players = this.Data.Players.OrderByDescending(p => p.Points)
                .Take(100).Select(x => new PlayerValuableModel() 
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Points,
                    Weight = x.Price,
                    Position = (int)x.Position,
                    Team = x.Team.Initials
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();
            //var bestTeamByPoints = this.Data.Players.Where(p => bestTeam.Any(x => x.Id == p.Id));

            return PartialView("_ValuableTeam", bestTeam);
        }

        public ActionResult TeamByPointsPerGame()
        {
            return View();
        }

        public ActionResult GetTeamByPointsPerGame(SubmitTeamPrice price)
        {
            var players = this.Data.Players.OrderByDescending(p => p.PointsPerGame)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.PointsPerGame,
                    Weight = x.Price,
                    Position = (int)x.Position,
                    Team = x.Team.Initials
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();

            return PartialView("_ValuableTeam", bestTeam);
        }

        public ActionResult TeamByPPP()
        {
            return View();
        }

        public ActionResult GetTeamByPPP(SubmitTeamPrice price)
        {
            var players = this.Data.Players.OrderByDescending(p => p.Points / p.Price)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Points / x.Price,
                    Weight = x.Price,
                    Position = (int)x.Position,
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
            var players = this.Data.Players.OrderByDescending(x => x.PointsPerGame / x.Price)
                .Take(100).Select(x => new PlayerValuableModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.PointsPerGame / x.Price,
                    Weight = x.Price,
                    Position = (int)x.Position,
                    Team = x.Team.Initials
                }).ToArray();

            Knapsack knapsack = new Knapsack(players, price.TeamPrice);
            knapsack.KnapsackProblem();
            var bestTeam = knapsack.OutputSolution().OrderBy(p => p.Position).ToList();

            return PartialView("_ValuableTeam", bestTeam);
        }
    }
}