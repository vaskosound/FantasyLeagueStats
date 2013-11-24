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
        public DataJob()
        { }
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            ExternalData statistics = (ExternalData) dataMap["data"];
            DataManager dataManager = (DataManager) dataMap["dataManager"];
            UowData data = (UowData) dataMap["dbContext"];
            List<string> stats = statistics.GetBasicStats();
            dataManager.UpdateBasicData(stats);
            List<string> statsPointsPerGame = statistics.GetStatsByPointsPerGame();
            dataManager.UpdatePointsPerGameData(statsPointsPerGame);
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