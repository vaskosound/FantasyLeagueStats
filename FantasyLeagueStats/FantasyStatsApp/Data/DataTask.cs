using FantasyStats.Data;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Data
{
    public class DataTask : ITask
    {
        protected IUowData Data { get; set; }

        public ExternalData Statistics { get; set; }

        public DataManager DataManager { get; set; }

        public DataTask(IUowData data)
        {
            this.Statistics = new ExternalData();
            this.DataManager = new DataManager();
            this.Data = data;
        }
        public void Execute()
        {
            List<string> stats = this.Statistics.GetBasicStats();
            this.DataManager.UpdateBasicData(stats);
            List<string> statsPointsPerGame = this.Statistics.GetStatsByPointsPerGame();
            this.DataManager.UpdatePointsPerGameData(statsPointsPerGame);
            List<string> statsLeagueTable = this.Statistics.GetStandings();
            this.DataManager.UpdateStandings(statsLeagueTable);

            DateTime currentDate = DateTime.Now;
            var currentGameweek = this.Data.Gameweeks.All()
                .FirstOrDefault(g => g.StartDate <= currentDate && currentDate <= g.EndDate);

            List<string> fixtures = this.Statistics.GetGameweek(currentGameweek.Id);
            this.DataManager.UpdateFixtures(fixtures);
        }
    }
}