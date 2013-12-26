using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models.GameModels
{
    public class PlayerGameViewModel
    {
        public static Expression<Func<PlayersGame, PlayerGameViewModel>> FromPlayersGame =
           player => new PlayerGameViewModel()
           {
               Id = player.PlayerId,
               Name = player.Player.Name,
               Team = player.Player.Team.Initials,
               Price = player.Player.Price,
               Position = player.Player.Position,
               IsStarting = player.IsStarting,
               RoundScore = player.Player.RoundScore,
           };
        public int Id { get; set; }

        public string Name { get; set; }

        public string Team { get; set; }

        public Position Position { get; set; }

        public decimal Price { get; set; }

        public int Points { get; set; }

        public int RoundScore { get; set; }

        public bool IsStarting { get; set; }

        public PlayerGameViewModel()
        { 
            
        }
    }
}