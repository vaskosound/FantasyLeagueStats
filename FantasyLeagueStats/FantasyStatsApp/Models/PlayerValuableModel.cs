﻿using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class PlayerValuableModel
    {      
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Value { get; set; }

        public decimal Weight { get; set; }

        public int Position { get; set; }

        public string Team { get; set; }
    }
}