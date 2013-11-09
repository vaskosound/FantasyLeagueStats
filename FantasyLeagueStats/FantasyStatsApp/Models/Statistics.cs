using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool acceptNextAlert = true;

        //public static void StatisticsPath()
        //{
        //    driver.Navigate().GoToUrl(baseURL + "/");
        //    driver.FindElement(By.Id("ismEmail")).Clear();
        //    driver.FindElement(By.Id("ismEmail")).SendKeys("vas_laz@abv.bg");
        //    driver.FindElement(By.Id("id_password")).Clear();
        //    driver.FindElement(By.Id("id_password")).SendKeys("vas20404");
        //    driver.FindElement(By.XPath("//input[@value='Log In']")).Click();
        //    driver.Navigate().GoToUrl(statsURL);

        //}

        public string GetBasicStats()
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            driver.Navigate().GoToUrl(statsMinutesURL);
            element = driver.FindElement(By.XPath("//table[@class='ismTable']"));
            string stats = element.Text;
            stats = DeletFirstRow(stats);
            stats += GetStatsFromNextPage(driver, statsPageMinutesURL + 2);        
            stats += GetStatsFromNextPage(driver, statsPageMinutesURL + 3);
            stats += GetStatsFromNextPage(driver, statsPageMinutesURL + 4);
            stats += GetStatsFromNextPage(driver, statsPageMinutesURL + 5);
            driver.Close();
            IWebDriver driverMinutes = new FirefoxDriver();
            driverMinutes.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            stats += GetStatsFromNextPage(driverMinutes, statsPageMinutesURL + 6);
            stats += GetStatsFromNextPage(driverMinutes, statsPageMinutesURL + 7);
            stats += GetStatsFromNextPage(driverMinutes, statsPageMinutesURL + 8);
            stats += GetStatsFromNextPage(driverMinutes, statsPageMinutesURL + 9);
            stats += GetStatsFromNextPage(driverMinutes, statsPageMinutesURL + 10);
            driverMinutes.Close();
            return stats;
        }

        public string GetStatsByPointsPerGame()
        {
            IWebDriver driverPointPerPrice = new FirefoxDriver();
            driverPointPerPrice.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            driverPointPerPrice.Navigate().GoToUrl(statsPointsPerGameURL);
            element = driverPointPerPrice.FindElement(By.XPath("//table[@class='ismTable']"));
            string stats = element.Text;
            stats = DeletFirstRow(stats);
            stats += GetStatsFromNextPage(driverPointPerPrice, statsPagePointsPerGameURL + 2);
            stats += GetStatsFromNextPage(driverPointPerPrice, statsPagePointsPerGameURL + 3);
            stats += GetStatsFromNextPage(driverPointPerPrice, statsPagePointsPerGameURL + 4);
            stats += GetStatsFromNextPage(driverPointPerPrice, statsPagePointsPerGameURL + 5);
            driverPointPerPrice.Close();
            IWebDriver driverPointPerPrice2 = new FirefoxDriver();
            driverPointPerPrice2.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            stats += GetStatsFromNextPage(driverPointPerPrice2, statsPagePointsPerGameURL + 6);
            stats += GetStatsFromNextPage(driverPointPerPrice2, statsPagePointsPerGameURL + 7);
            stats += GetStatsFromNextPage(driverPointPerPrice2, statsPagePointsPerGameURL + 8);
            stats += GetStatsFromNextPage(driverPointPerPrice2, statsPagePointsPerGameURL + 9);
            stats += GetStatsFromNextPage(driverPointPerPrice2, statsPagePointsPerGameURL + 10);
            driverPointPerPrice2.Close();
            return stats;
        }

        private string GetStatsFromNextPage(IWebDriver dr, string pageUrl)
        {
            dr.Navigate().GoToUrl(pageUrl);
            element = dr.FindElement(By.XPath("//table[@class='ismTable']"));
            string stats = element.Text;
            stats = DeletFirstRow(stats);
            return stats;
        }
        public string DeletFirstRow(string stats)
        {
            int index = stats.IndexOf("\n");
            return stats = stats.Substring(index);
        }
    }
}