﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Models.GameModels
{
    public class GameResultsViewModel
    {
        public string FirstUser { get; set; }
        
        public SortedDictionary<int, List<PlayerGameViewModel>> FirstUserPlayers { get; set; }

        public string SecondUser { get; set; }

        public SortedDictionary<int, List<PlayerGameViewModel>> SecondUserPlayers { get; set; }
    }
}