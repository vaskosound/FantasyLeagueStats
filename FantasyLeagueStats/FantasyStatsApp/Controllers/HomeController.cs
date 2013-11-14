using FantasyStats.Data;
using FantasyStatsApp.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;

namespace FantasyStatsApp.Controllers
{
    public class HomeController : Controller
    {
        public ExternalData Statistics { get; set; }

        public ApplicationDbContext Data { get; set; }
        public HomeController()
        {
            this.Statistics = new ExternalData();
            this.Data = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ReadStandings([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Teams.OrderBy(x => x.Position).Select(TeamViewModel.FromTeams);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
             
    }
}