using FantasyStats.Data;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using FantasyStatsApp.Models;

namespace FantasyStatsApp.Controllers
{
    public class StatisticsController : Controller
    {
        //
        // GET: /Statistics/
        public ApplicationDbContext Data { get; set; }

        public StatisticsController()
        {
            this.Data = new ApplicationDbContext();
        }
        public ActionResult Goalkeeprs()
        {
            return View();
        }

        public JsonResult ReadGoalkeepers([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.Where(x => (int)x.Position == 0)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Defenders()
        {
            return View();
        }

        public JsonResult ReadDefenders([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.Where(x => (int)x.Position == 1)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Midfielders()
        {
            return View();
        }

        public JsonResult ReadMidfielders([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.Where(x => (int)x.Position == 2)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Forwards()
        {
            return View();
        }

        public JsonResult ReadForwards([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.Where(x => (int)x.Position == 3)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllPositions()
        {
            return View();
        }

        public JsonResult ReadAllPositions([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.Select(PlayerViewModel.FromPlayers)
                .OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
	}
}