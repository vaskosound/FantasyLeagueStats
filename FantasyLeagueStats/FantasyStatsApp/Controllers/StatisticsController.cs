using FantasyStats.Data;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using FantasyStatsApp.Models;
using FantasyStats.Model;

namespace FantasyStatsApp.Controllers
{
    [Authorize]
    public class StatisticsController : BaseController
    {
        public ActionResult Goalkeeprs()
        {
            return View();
        }

        public JsonResult ReadGoalkeepers([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.All().Where(x => x.Position == Position.GKP)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Defenders()
        {
            return View();
        }

        public JsonResult ReadDefenders([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.All().Where(x => x.Position == Position.DEF)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Midfielders()
        {
            return View();
        }

        public JsonResult ReadMidfielders([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.All().Where(x => x.Position == Position.MID)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Forwards()
        {
            return View();
        }

        public JsonResult ReadForwards([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.All().Where(x => x.Position == Position.FWD)
                .Select(PlayerViewModel.FromPlayers).OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllPositions()
        {
            return View();
        }

        public JsonResult ReadAllPositions([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.All().Select(PlayerViewModel.FromPlayers)
                .OrderByDescending(x => x.PointsPerPrice);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
	}
}