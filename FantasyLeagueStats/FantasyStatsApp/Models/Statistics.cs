using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class Statistics
    {
        private const string baseURL = "http://fantasy.premierleague.com";
        private const string statsURL = baseURL + "/stats/elements/";
        private const string statsMinutesURL = statsURL + "/?element_filter=0&stat_filter=minutes";
        private const string statsPointsPerGameURL = statsURL + "/?element_filter=0&stat_filter=points_per_game";
        private const string statsPageMinutesURL = statsURL + "?stat_filter=minutes&element_filter=0&page=";
        private const string statsPagePointsPerGameURL = statsURL + "?stat_filter=points_per_game&element_filter=0&page=";
        private const string statsLeagueTable = "http://www.premierleague.com/content/premierleague/en-gb/matchday/league-table.html";
        private const string currentFixtures = baseURL + "/fixtures/";
        private const string rules = baseURL + "/rules/";

        public List<string> GetBasicStats()
        {
            List<string> stats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                stats.AddRange(GetStatsFromTable(client, statsMinutesURL));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 2));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 3));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 4));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 5));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 6));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 7));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 8));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 9));
                stats.AddRange(GetStatsFromTable(client, statsPageMinutesURL + 10));
            }

            return stats;
        }
        public List<string> GetStatsByPointsPerGame()
        {
            List<string> stats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                stats.AddRange(GetStatsFromTable(client, statsPointsPerGameURL));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 2));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 3));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 4));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 5));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 6));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 7));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 8));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 9));
                stats.AddRange(GetStatsFromTable(client, statsPagePointsPerGameURL + 10));

            }

            return stats;
        }

        private List<string> GetStatsFromTable(WebClient client, string pageUrl)
        {
            string html = client.DownloadString(pageUrl);
            string pattern = "(<td>)(.*)(<\\/td>)";
            var matches = Regex.Matches(html, pattern);
            List<string> stats = new List<string>();
            foreach (Match item in matches)
            {
                if (!item.Groups[2].Value.Contains("<"))
                {
                    stats.Add(item.Groups[2].Value);
                }
            }
            return stats;
        }

        public List<string> GetStandings()
        {
            List<string> standings = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                string html = client.DownloadString(statsLeagueTable);
                string pattern = "(<td.*>)(.*)(<\\/td>)";
                var matches = Regex.Matches(html, pattern);
                foreach (Match item in matches)
                {
                    standings.Add(item.Groups[2].Value);
                }
            }

            return standings;
        }

        public List<string> GetFixtures()
        {
            List<string> fixtures = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                string html = client.DownloadString(currentFixtures);
                int gameweekIndex = html.IndexOf("ismStrongCaption");
                string fixtureGameweek = html.Substring(gameweekIndex);
                int endGameweekIndex = fixtureGameweek.IndexOf("-");
                var gameweek = fixtureGameweek.Substring(18, endGameweekIndex - 19);
                fixtures.Add(gameweek);
                int endIndex = fixtureGameweek.IndexOf("ismNoJsMsg");
                int startIndex = endGameweekIndex;
                string fixturesTable = fixtureGameweek.Substring(startIndex, endIndex - startIndex);
                string pattern = "(<td.*?>)(.*?)(<\\/td>)";
                var matches = Regex.Matches(fixturesTable, pattern);
                foreach (Match item in matches)
                {
                    if (!item.Groups[2].Value.Contains("<"))
                    {
                        fixtures.Add(item.Groups[2].Value);
                    }
                }
            }

            return fixtures;
        }

        public List<string> GetGameweeks()
        {
            List<string> gamewеeks = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                string html = client.DownloadString(rules);
                int startIndex = html.IndexOf("All changes to your team");
                int endIndex = html.IndexOf("During the season");
                string gameweeksList = html.Substring(startIndex, endIndex - startIndex);
                string pattern = "(<td>)(.*)(<\\/td>)";
                var matches = Regex.Matches(gameweeksList, pattern);
                foreach (Match item in matches)
                {
                    gamewеeks.Add(item.Groups[2].Value);
                }
            }

            return gamewеeks;
        }
    }
}