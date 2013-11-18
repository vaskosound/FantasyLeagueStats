using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class PlayerBasicModel
    {
        public static Expression<Func<Player, PlayerBasicModel>> FromPlayersStats =
           player => new PlayerBasicModel()
           {
               Id = player.Id,
               Name = player.Name,
               Team = player.Team.Initials,
               Selected = player.Selected,
               Price = player.Price,
               Position = player.Position,
               Points = player.Points,
               MinutesPlayed = player.MinutesPlayed,
               PointsPerGame = player.PointsPerGame
           };
        public int Id { get; set; }

        public string Name { get; set; }

        public string Team { get; set; }

        public double Selected { get; set; }

        public decimal Price { get; set; }

        public int Points { get; set; }
                
        public Position Position { get; set; }

        public string PositionName
        {
            get
            {
                return this.Position.ToString();
            }
        }

        public int MinutesPlayed { get; set; }

        public decimal PointsPerGame { get; set; }
    }
}