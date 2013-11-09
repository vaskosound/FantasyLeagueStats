using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class PlayerViewModel
    {
        public static Expression<Func<Player, PlayerViewModel>> FromPlayers =
            player => new PlayerViewModel()
            {
                Id = player.Id,
                Name = player.Name,
                Team = player.Team.Initials,
                Selected = player.Selected,
                Price = player.Price,
                Position = player.Position,
                Points = player.Points,
                PointsPerPrice = Math.Round(player.Points / player.Price, 2),
                PPPPerMinutes = Math.Round(((player.Points / player.Price) / player.MinutesPlayed) * 100, 4),
                PPPPerGame = player.PointsPerGame
            };
        public int Id { get; set; }

        public string Name { get; set; }

        public string Team { get; set; }

        public double Selected { get; set; }

        public decimal Price { get; set; }

        public int Points { get; set; }

        public decimal PointsPerPrice { get; set; }

        public decimal PPPPerMinutes { get; set; }

        public decimal PPPPerGame { get; set; }

        public Position Position { get; set; }

        public string PositionName
        {
            get
            {
                return this.Position.ToString();
            }
        }
    }
}