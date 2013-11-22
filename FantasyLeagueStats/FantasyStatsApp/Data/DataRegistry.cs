using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Data
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            Schedule<DataTask>().ToRunEvery(2).Hours();
        }
    }
}