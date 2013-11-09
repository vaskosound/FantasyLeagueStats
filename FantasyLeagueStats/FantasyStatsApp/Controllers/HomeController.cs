using FantasyStats.Data;
using FantasyStatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FantasyStatsApp.Controllers
{
    public class HomeController : Controller
    {
        public Statistics Statistics { get; set; }
        public HomeController()
        {
            this.Statistics = new Statistics();
        }
        public ActionResult Index()
        {
            return View();
        }
             
        public ActionResult UpdateData()
        {
            //Statistics.StatisticsPath();
            string stats = this.Statistics.GetBasicStats();
            MoneyBall.UpdateBasicData(stats);
            string statsPointsPerGame = this.Statistics.GetStatsByPointsPerGame();
            MoneyBall.UpdatePointsPerGameData(statsPointsPerGame);
            return View();
        }
    }
}