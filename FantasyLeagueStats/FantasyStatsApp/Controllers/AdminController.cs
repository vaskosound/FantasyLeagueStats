﻿using FantasyStatsApp.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using FantasyStats.Model;
using FantasyStatsApp.Data;
using Quartz;
using Quartz.Impl;
using FantasyStats.Data;
using Quartz.Impl.Triggers;

namespace FantasyStatsApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private static IScheduler scheduler;
        public ExternalData Statistics { get; set; }

        public DataManager DataManager { get; set; }

        public AdminController()
        {
            this.Statistics = new ExternalData();
            this.DataManager = new DataManager();
        }


        public ActionResult PlayersStats()
        {
            return View(scheduler);
        }

        public JsonResult ReadPlayersStats([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Data.Players.All().Select(PlayerBasicModel.FromPlayersStats);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StartSchedule()
        {
            ISchedulerFactory schedFactory = new StdSchedulerFactory();

            scheduler = schedFactory.GetScheduler();
            scheduler.Start();
            IJobDetail jobDetail = new JobDetailImpl("myJob", null, typeof(DataJob));
            jobDetail.JobDataMap["data"] = new ExternalData();
            jobDetail.JobDataMap["dataManager"] = new DataManager();
            jobDetail.JobDataMap["dbContext"] = new UowData();
            ISimpleTrigger trigger = new SimpleTriggerImpl("myTrigger", null, DateTime.UtcNow.AddYears(1),
                null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromHours(2));
            scheduler.ScheduleJob(jobDetail, trigger);
            
            return PartialView("_Scheduler", scheduler);
        }

        public ActionResult StopSchedule()
        {         
            scheduler.Shutdown();

            return PartialView("_Scheduler", scheduler);
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
            var result = this.Data.Matches.All().OrderBy(m => m.MatchDate)
                .Select(MatchViewModel.FromMatches);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateMatch([DataSourceRequest] DataSourceRequest request, MatchViewModel match)
        {
            if (match != null && ModelState.IsValid)
            {
                var hostTeam = this.Data.Teams.All().FirstOrDefault(x => x.Name == match.Host);
                var visitorTeam = this.Data.Teams.All().FirstOrDefault(x => x.Name == match.Visitor);
                var gameweek = this.Data.Gameweeks.All().FirstOrDefault(g => g.Name == match.Gameweek);
                var matchExists = this.Data.Matches.All().FirstOrDefault(m => m.Host.Name == match.Host &&
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
            var editedMatch = this.Data.Matches.GetById(match.Id);
            var hostTeam = this.Data.Teams.All().FirstOrDefault(x => x.Name == match.Host);
            var visitorTeam = this.Data.Teams.All().FirstOrDefault(x => x.Name == match.Visitor);
            var gameweek = this.Data.Gameweeks.All().FirstOrDefault(g => g.Name == match.Gameweek);

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
            var deletedMatch = this.Data.Matches.GetById(match.Id);

            this.Data.Matches.Delete(deletedMatch);
            this.Data.SaveChanges();

            return Json(new[] { match }.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpadateResults()
        {
            DateTime currentDate = DateTime.Now;
            var currentGameweek = this.Data.Gameweeks.All()
                .FirstOrDefault(g => g.StartDate <= currentDate && currentDate <= g.EndDate);

            List<string> fixtures = this.Statistics.GetGameweek(currentGameweek.Id);
            this.DataManager.UpdateFixtures(fixtures);

            return PartialView("_MatchesGrid");
        }

        public ActionResult UpadateFixtures()
        {
            var gameweeks = this.Data.Gameweeks.All();
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
            var teams = dataContext.Teams.All().OrderBy(e => e.Name)
                        .Select(SelectTeamModel.FromTeams);

            ViewData["teams"] = teams;
        }

        private void PopulateGameweek()
        {
            var dataContext = this.Data;
            var gameweeks = dataContext.Gameweeks.All().OrderBy(e => e.Id)
                        .Select(GameweekModel.FromGameweeks);

            ViewData["gameweeks"] = gameweeks;
        }
    }
}