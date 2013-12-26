using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Models.GameModels
{
    public class GameBasicViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public GameState GameState { get; set; }
    }
}