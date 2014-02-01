using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FantasyStats.Model
{
    public class Match
    {
        public int Id { get; set; }

        [DisplayName("HostId")]
        public int? Host_TeamId  { get; set; }

        [ForeignKey(" Host_TeamId"), Column(Order = 0)]
        public virtual Team Host { get; set; }

        [DisplayName("VisitorId")]
        public int? Visitor_TeamId { get; set; }

        [ForeignKey(" Visitor_TeamId"), Column(Order = 1)]
        public virtual Team Visitor { get; set; }

        public int? HostScore { get; set; }

        public int? VistorScore { get; set; }

        public int GameweekId { get; set; }

        public virtual Gameweek Gameweek { get; set; }

        public DateTime MatchDate { get; set; }
    }
}
