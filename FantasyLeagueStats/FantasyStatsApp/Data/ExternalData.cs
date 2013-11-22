using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FantasyStatsApp.Data
{
    public class ExternalData
    {
        private const string Base_URL = "http://fantasy.premierleague.com";
        private const string Stats_URL = Base_URL + "/stats/elements/";
        private const string Stats_Minutes_URL = Stats_URL + "/?element_filter=0&stat_filter=minutes";
        private const string Stats_Points_Per_Game_URL = Stats_URL + "/?element_filter=0&stat_filter=points_per_game";
        private const string Stats_Page_Minutes_URL = Stats_URL + "?stat_filter=minutes&element_filter=0&page=";
        private const string Stats_Page_Points_Per_Game_URL = Stats_URL + "?stat_filter=points_per_game&element_filter=0&page=";
        private const string Stats_League_Table = "http://www.premierleague.com/content/premierleague/en-gb/matchday/league-table.html";
        private const string Current_Fixtures = Base_URL + "/fixtures/";
        private const string Rules = Base_URL + "/rules/";

        public List<string> GetBasicStats()
        {
            List<string> stats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                stats.AddRange(GetStatsFromTable(client, Stats_Minutes_URL));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 2));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 3));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 4));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 5));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 6));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 7));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 8));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 9));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Minutes_URL + 10));
            }

            return stats;
        }
        public List<string> GetStatsByPointsPerGame()
        {
            List<string> stats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                stats.AddRange(GetStatsFromTable(client, Stats_Points_Per_Game_URL));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 2));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 3));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 4));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 5));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 6));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 7));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 8));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 9));
                stats.AddRange(GetStatsFromTable(client, Stats_Page_Points_Per_Game_URL + 10));

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
                string html = client.DownloadString(Stats_League_Table);
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
                fixtures = GetGameweekFixture(client, Current_Fixtures + gameweek);
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

        public List<string> GetGameweeks()
        {
            List<string> gamewеeks = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                string html = client.DownloadString(Rules);
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