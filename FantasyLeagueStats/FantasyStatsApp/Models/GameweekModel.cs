﻿using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class GameweekModel
    {
        public static Expression<Func<Gameweek, GameweekModel>> FromGameweeks =
         gameweek => new GameweekModel()
         {
             Id = gameweek.Id,
             Name = gameweek.Name
         };
        public int Id { get; set; }

        public string Name { get; set; }
    }
}