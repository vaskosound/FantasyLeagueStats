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
            var currentMatches = PopulateCurrentMatches();

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
                .OrderBy(d => d.MatchDate).Select(MatchViewModel.FromMatches);

            return PartialView("_Fixtures", gameweekFixtures);
        }
    }
}