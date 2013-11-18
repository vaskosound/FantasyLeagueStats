using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class PlayerViewModel : PlayerBasicModel
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
                PointsPerGame = player.PointsPerGame,
                PointsPerPrice = Math.Round(player.Points / player.Price, 2),
                PPPPerMinutes = Math.Round(((player.Points / player.Price) / player.MinutesPlayed) * 100, 4),
                PPPPerGame = Math.Round(player.PointsPerGame / player.Price, 4)
            };
        public decimal PointsPerPrice { get; set; }

        public decimal PPPPerMinutes { get; set; }

        public decimal PPPPerGame { get; set; }        
    }
}