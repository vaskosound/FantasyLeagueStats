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
        private IWebElement element;
        private const string baseURL = "http://fantasy.premierleague.com";
        private const string statsURL = baseURL + "/stats/elements/";
        private const string statsMinutesURL = statsURL + "/?element_filter=0&stat_filter=minutes";
        private const string statsPointsPerGameURL = statsURL + "/?element_filter=0&stat_filter=points_per_game";
        private const string statsPageMinutesURL = statsURL + "?stat_filter=minutes&element_filter=0&page=";
        private const string statsPagePointsPerGameURL = statsURL + "?stat_filter=points_per_game&element_filter=0&page=";

        public List<string> GetBasicStats()
        {
            List<string> stats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                stats.AddRange(GetStatsFromNextPage(client, statsMinutesURL));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 2));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 3));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 4));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 5));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 6));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 7));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 8));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 9));
                stats.AddRange(GetStatsFromNextPage(client, statsPageMinutesURL + 10));
            }

            return stats;
        }
        public List<string> GetStatsByPointsPerGame()
        {
            List<string> stats = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                stats.AddRange(GetStatsFromNextPage(client, statsPointsPerGameURL));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 2));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 3));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 4));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 5));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 6));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 7));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 8));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 9));
                stats.AddRange(GetStatsFromNextPage(client, statsPagePointsPerGameURL + 10));

            }

            return stats;
        }

        private List<string> GetStatsFromNextPage(WebClient client, string pageUrl)
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
    }
}