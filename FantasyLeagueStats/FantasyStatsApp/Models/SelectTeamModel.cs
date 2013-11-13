using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class SelectTeamModel
    {
        public static Expression<Func<Team, SelectTeamModel>> FromTeams =
         team => new SelectTeamModel()
         {
             Id = team.Id,
             Name = team.Name
         };

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}