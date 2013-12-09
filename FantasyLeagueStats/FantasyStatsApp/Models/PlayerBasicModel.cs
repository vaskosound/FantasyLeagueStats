using FantasyStats.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

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
               RoundScore = player.RoundScore,
               IsInjured = player.IsInjured,
               MinutesPlayed = player.MinutesPlayed,
               PointsPerGame = player.PointsPerGame,
               PlayerForm = player.PlayerForm
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

        public bool IsInjured { get; set; }

        public int MinutesPlayed { get; set; }

        public decimal PointsPerGame { get; set; }

        public decimal PlayerForm { get; set; }

        public int RoundScore { get; set; }
    }
}