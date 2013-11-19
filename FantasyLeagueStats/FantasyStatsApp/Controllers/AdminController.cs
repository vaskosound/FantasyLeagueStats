using FantasyStats.Data;
using FantasyStatsApp.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using FantasyStats.Model;

namespace FantasyStatsApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ExternalData Statistics { get; set; }

        public DataManager DataManager { get; set; }
        public ApplicationDbContext Data { get; set; }

        public AdminController()
        {
            this.Statistics = new ExternalData();
            this.DataManager = new DataManager();
            this.Data = new ApplicationDbContext();
        }


        public ActionResult PlayersStats()
        {
            return View();
        }

        public JsonResult ReadPlayersStats([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.Select(PlayerBasicModel.FromPlayersStats);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateData()
        {
            List<string> stats = this.Statistics.GetBasicStats();
            this.DataManager.UpdateBasicData(stats);
            List<string> statsPointsPerGame = this.Statistics.GetStatsByPointsPerGame();
            this.DataManager.UpdatePointsPerGameData(statsPointsPerGame);
            List<string> statsLeagueTable = this.Statistics.GetStandings();
            this.DataManager.UpdateStandings(statsLeagueTable);

            return PartialView("_PlayersStatsGrid");
        }

        public ActionResult Matches()
        {
            PopulateTeams();
            PopulateGameweek();
            return View();
        }

        public JsonResult ReadMatches([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Matches.OrderBy(m => m.MatchDate).Select(MatchViewModel.FromMatches);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateMatch([DataSourceRequest] DataSourceRequest request, MatchViewModel match)
        {
            if (match != null && ModelState.IsValid)
            {
                var hostTeam = this.Data.Teams.FirstOrDefault(x => x.Name == match.Host);
                var visitorTeam = this.Data.Teams.FirstOrDefault(x => x.Name == match.Visitor);
                var gameweek = this.Data.Gameweeks.FirstOrDefault(g => g.Name == match.Gameweek);
                var matchExists = this.Data.Matches.FirstOrDefault(m => m.Host.Name == match.Host &&
                    m.Visitor.Name == match.Visitor);

                if (matchExists == null)
                {
                    var newMatch = new Match
                    {
                        Host = hostTeam,
                        Visitor = visitorTeam,
                        HostScore = match.HostScore,
                        VistorScore = match.VisitorScore,
                        Gameweek = gameweek,
                        MatchDate = match.MatchDate
                    };

                    this.Data.Matches.Add(newMatch);
                    this.Data.SaveChanges();
                }
                match.Id = match.Id;
            }

            return Json(new[] { match }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditMatch([DataSourceRequest] DataSourceRequest request, MatchViewModel match)
        {
            var editedMatch = this.Data.Matches.Find(match.Id);
            var hostTeam = this.Data.Teams.FirstOrDefault(x => x.Name == match.Host);
            var visitorTeam = this.Data.Teams.FirstOrDefault(x => x.Name == match.Visitor);
            var gameweek = this.Data.Gameweeks.FirstOrDefault(g => g.Name == match.Gameweek);

            editedMatch.Gameweek = gameweek;
            editedMatch.Host = hostTeam;
            editedMatch.Visitor = visitorTeam;
            editedMatch.HostScore = match.HostScore;
            editedMatch.VistorScore = match.VisitorScore;
            editedMatch.MatchDate = match.MatchDate;

            this.Data.SaveChanges();

            return Json(new[] { match }.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMatch([DataSourceRequest] DataSourceRequest request, MatchViewModel match)
        {
            var deletedMatch = this.Data.Matches.Find(match.Id);

            this.Data.Matches.Remove(deletedMatch);
            this.Data.SaveChanges();

            return Json(new[] { match }.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpadateResults()
        {
            DateTime currentDate = DateTime.Now;
            var currentGameweek = this.Data.Gameweeks
                .FirstOrDefault(g => g.StartDate <= currentDate && currentDate <= g.EndDate);
            List<string> fixtures = this.Statistics.GetGameweek(currentGameweek.Id);
            this.DataManager.UpdateFixtures(fixtures);

            return PartialView("_MatchesGrid");
        }

        public ActionResult UpadateFixtures()
        {
            var gameweeks = this.Data.Gameweeks;
            if (gameweeks.Count() == 0)
            {
                for (int i = 1; i <= 38; i++)
                {
                    List<string> fixtures = this.Statistics.GetGameweek(i);
                    this.DataManager.UpdateFixtures(fixtures);
                }
            }
            else
            {
                DateTime currentDate = DateTime.Now;
                var remainingGameweeks = gameweeks.Where(g => g.StartDate >= currentDate);
                foreach (var gameweek in remainingGameweeks)
                {
                    List<string> fixtures = this.Statistics.GetGameweek(gameweek.Id);
                    this.DataManager.UpdateFixtures(fixtures);
                }
            }

            return PartialView("_MatchesGrid");
        }

        private void PopulateTeams()
        {
            var dataContext = this.Data;
            var teams = dataContext.Teams
                        .OrderBy(e => e.Name)
                        .Select(SelectTeamModel.FromTeams);

            ViewData["teams"] = teams;
        }

        private void PopulateGameweek()
        {
            var dataContext = this.Data;
            var gameweeks = dataContext.Gameweeks
                        .OrderBy(e => e.Id)
                        .Select(GameweekModel.FromGameweeks);

            ViewData["gameweeks"] = gameweeks;
        }
    }
}