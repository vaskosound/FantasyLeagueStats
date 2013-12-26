using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class SelectPlayerModel
    {
        public static Expression<Func<Player, SelectPlayerModel>> FromPlayers =
         player => new SelectPlayerModel()
         {
             Id = player.Id,
             Name = player.Name
         };
        public int Id { get; set; }

        public string Name { get; set; }
    }
}