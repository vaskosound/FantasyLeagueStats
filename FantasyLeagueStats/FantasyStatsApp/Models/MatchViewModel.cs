using FantasyStats.Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;


namespace FantasyStatsApp.Models
{
    public class MatchViewModel
    {
        public static Expression<Func<Match, MatchViewModel>> FromMatches =
          match => new MatchViewModel()
          {
              Id = match.Id,
              Host = match.Host.Name,
              HostInitials = match.Host.Initials,
              Visitor = match.Visitor.Name,
              VisitorInitials = match.Visitor.Initials,
              HostScore = match.HostScore,
              VisitorScore = match.VistorScore,
              Gameweek = match.Gameweek.Name,
              MatchDate = match.MatchDate,
          };

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [UIHint("EditorTeam")]
        public string Host { get; set; }

        public string HostInitials { get; set; }

        [UIHint("EditorTeam")]
        public string Visitor { get; set; }

        public string VisitorInitials { get; set; }

        public int? HostScore { get; set; }

        public int? VisitorScore { get; set; }

        [UIHint("EditorGameweek")]
        public string Gameweek { get; set; }

        [UIHint("EditorMatchDate")]
        public DateTime MatchDate { get; set; }    
    }
}