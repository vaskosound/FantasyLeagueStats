using FantasyStatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FantasyStatsApp.Controllers
{
    public class AdminController : Controller
    {
        public Statistics Statistics { get; set; }
        
        public AdminController()
        {
            this.Statistics = new Statistics();
        }
        
        public ActionResult UpdateData()
        {
            List<string> stats = this.Statistics.GetBasicStats();
            MoneyBall.UpdateBasicData(stats);
            List<string> statsPointsPerGame = this.Statistics.GetStatsByPointsPerGame();
            MoneyBall.UpdatePointsPerGameData(statsPointsPerGame);
            List<string> statsLeagueTable = this.Statistics.GetStandings();
            MoneyBall.UpdateStandings(statsLeagueTable);
            return View();
        }
	}
}