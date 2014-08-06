using FantasyStats.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Data
{
    public class DataJob : IJob
    {
        private const int PAGE_COUNT = 20;

        public DataJob()
        { }
        public void Execute(IJobExecutionContext context)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            ExternalData statistics = (ExternalData) dataMap["data"];
            DataManager dataManager = (DataManager) dataMap["dataManager"];
            UowData data = (UowData) dataMap["dbContext"];

            List<string> stats = new List<string>();
            List<string> statsPointsPerGame = new List<string>();
            List<string> statsForm = new List<string>();

            for (int i = 1; i <= PAGE_COUNT; i++)
            {
                stats.AddRange(statistics.GetBasicStats(i));
                statsPointsPerGame.AddRange(statistics.GetStatsByPointsPerGame(i));
                statsForm.AddRange(statistics.GetStatsByForm(i));
            }
            dataManager.UpdateBasicData(stats);
            dataManager.UpdatePointsPerGameData(statsPointsPerGame);
            dataManager.UpdatePlayersForm(statsForm);

            List<string> statsLeagueTable = statistics.GetStandings();
            dataManager.UpdateStandings(statsLeagueTable);

            DateTime currentDate = DateTime.Now;
            var currentGameweek = data.Gameweeks.All()
                .FirstOrDefault(g => g.StartDate <= currentDate && currentDate <= g.EndDate);

            List<string> fixtures = statistics.GetGameweek(currentGameweek.Id);
            dataManager.UpdateFixtures(fixtures);
        }
    }
}