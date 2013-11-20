using FantasyStats.Model;
using System;
using System.Linq;

namespace FantasyStatsApp.Models
{
    public class PlayerValuableModel
    {      
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Value { get; set; }

        public decimal Weight { get; set; }

        public Position Position { get; set; }

        public string Team { get; set; }
    }
}