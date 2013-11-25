using FantasyStats.Data;
using FantasyStatsApp.Data;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;

namespace FantasyStatsApp
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            app.UseFacebookAuthentication(
               appId: "178656178997378",
               appSecret: "927ad51eedb97b67e93a24915a6207a9");

            //app.UseGoogleAuthentication();
        }

        public void StartSchedule()
        {
            ISchedulerFactory schedFactory = new StdSchedulerFactory();

            IScheduler scheduler = schedFactory.GetScheduler();
            scheduler.Start();
            IJobDetail jobDetail = new JobDetailImpl("myJob", null, typeof(DataJob));
            jobDetail.JobDataMap["data"] = new ExternalData();
            jobDetail.JobDataMap["dataManager"] = new DataManager();
            jobDetail.JobDataMap["dbContext"] = new UowData();
            ISimpleTrigger trigger = new SimpleTriggerImpl("myTrigger", null, DateTime.UtcNow,
                DateTime.UtcNow.AddYears(1), SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromHours(2));
            scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}