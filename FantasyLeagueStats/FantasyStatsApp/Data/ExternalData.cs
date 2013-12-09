using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FantasyStatsApp.Data
{
    public class ExternalData
    {
        private const string BASE_URL = "http://fantasy.premierleague.com";
        private const string STATS_URL = BASE_URL + "/stats/elements/";
        private const string STATS_PAGE_MINUTES_URL = STATS_URL + "?stat_filter=minutes&element_filter=0&page=";
        private const string STATS_PAGE_POINTS_PER_GAME_URL = STATS_URL + "?stat_filter=points_per_game&element_filter=0&page=";
        private const string STATS_PAGE_FORM_URL = STATS_URL + "?stat_filter=form&element_filter=0&page=";
        private const string STATS_PAGE_ROUND_SCORE_URL = STATS_URL + "?stat_filter=event_points&element_filter=0&page=";
        private const string STATS_LEAGUE_TABLE_URL = "http://www.premierleague.com/content/premierleague/en-gb/matchday/league-table.html";
        private const string CURRENT_FIXTURES_URL = BASE_URL + "/fixtures/";
        private const string RULES_URL = BASE_URL + "/rules/";

        public List<string> GetBasicStats(int page)
        {
            List<string> pageStats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                pageStats = GetStatsFromTable(client, STATS_PAGE_MINUTES_URL + page);
            }

            return pageStats;
        }

        public List<string> GetStatsByPointsPerGame(int page)
        {
            List<string> pageStats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                pageStats = GetStatsFromTable(client, STATS_PAGE_POINTS_PER_GAME_URL + page);
            }

            return pageStats;
        }
        public List<string> GetStatsByForm(int page)
        {
            List<string> pageStats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                pageStats = GetStatsFromTable(client, STATS_PAGE_FORM_URL + page);
            }

            return pageStats;
        }

        public List<string> GetStatsByRoundScore(int page)
        {
            List<string> pageStats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                pageStats = GetStatsFromTable(client, STATS_PAGE_ROUND_SCORE_URL + page);
            }

            return pageStats;
        }

        private List<string> GetStatsFromTable(WebClient client, string pageUrl)
        {
            string html = client.DownloadString(pageUrl);
            string pattern = "(<td>)(.*)(<\\/td>)";
            var matches = Regex.Matches(html, pattern);
            List<string> stats = new List<string>();
            foreach (Match item in matches)
            {
                if (!item.Groups[2].Value.StartsWith("<img") && !item.Groups[2].Value.Contains("<td>"))
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
                string html = client.DownloadString(STATS_LEAGUE_TABLE_URL);
                string pattern = "(<td.*>)(.*)(<\\/td>)";
                var matches = Regex.Matches(html, pattern);
                foreach (Match item in matches)
                {
                    standings.Add(item.Groups[2].Value);
                }
            }

            return standings;
        }

        public List<string> GetGameweek(int gameweek)
        {
            List<string> fixtures = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                fixtures = GetGameweekFixture(client, CURRENT_FIXTURES_URL + gameweek);
            }

            return fixtures;
        }

        private static List<string> GetGameweekFixture(WebClient client, string gameweekUrl)
        {
            List<string> fixtures = new List<string>();
            string html = client.DownloadString(gameweekUrl);
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

            return fixtures;
        }

        public List<string> GetGameweeksDeadline()
        {
            List<string> deadlines = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                string html = client.DownloadString(RULES_URL);
                int startIndex = html.IndexOf("<!-- DEADLINES -->");
                int endIndex = html.IndexOf("<!-- SCORING -->");
                string deadlinesTable = html.Substring(startIndex, endIndex - startIndex);
                string pattern = "(<td.*?>)(.*?)(<\\/td>)";
                var matches = Regex.Matches(deadlinesTable, pattern);
                foreach (Match item in matches)
                {
                    deadlines.Add(item.Groups[2].Value);
                }
            }

            return deadlines;
        }

    }
}