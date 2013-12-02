using FantasyStats.Model;
using System;
using System.Linq;

namespace FantasyStatsApp.Models
{
    public class PlayerModel
    {
        public string Name { get; set; }

        public string Team { get; set; }

        public Position Position { get; set; }

        public double Selected { get; set; }

        public int Points { get; set; }

        public decimal Price { get; set; }

        public bool IsInjured { get; set; }

        public int MinutesPlayed { get; set; }

        public decimal PointsPerGame { get; set; }

        public decimal PlayerForm { get; set; }

        public void SetPosition(string position)
        {
            switch (position)
            {
                case "GKP": this.Position = Position.GKP;
                    break;
                case "DEF": this.Position = Position.DEF;
                    break;
                case "MID": this.Position = Position.MID;
                    break;
                case "FWD": this.Position = Position.FWD;
                    break;
                default:
                    break;
            }
        }
        public void SetPrice(string playerPrice)
        {
            string price = playerPrice.Substring(1);
            this.Price = decimal.Parse(price);
        }
    }
}