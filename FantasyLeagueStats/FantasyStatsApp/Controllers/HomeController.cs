using FantasyStatsApp.Models;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using FantasyStatsApp.Data;
using FluentScheduler;

namespace FantasyStatsApp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            DateTime currentDate = DateTime.Now;
            var currentGameweek = this.Data.Gameweeks.All()
                .FirstOrDefault(g => g.StartDate <= currentDate && currentDate <= g.EndDate);

            var currentMatches = this.Data.Matches.All().Where(m => m.Gameweek.Id == currentGameweek.Id)
                .Select(MatchViewModel.FromMatches);

            return View(currentMatches);
        }

        public JsonResult ReadStandings([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Teams.All().OrderBy(x => x.Position).Select(TeamViewModel.FromTeams);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeGameweek(int gameweek)
        {
            var gameweekFixtures = this.Data.Matches.All().Where(m => m.Gameweek.Id == gameweek)
                .Select(MatchViewModel.FromMatches);

            return PartialView("_Fixtures", gameweekFixtures);
        }
    }
}