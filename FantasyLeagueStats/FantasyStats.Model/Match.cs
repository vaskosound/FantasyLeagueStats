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

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Match match = obj as Match;
            if ((Object)match == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Host_TeamId == match.Host_TeamId) && (Visitor_TeamId == match.Visitor_TeamId);
        }

        public bool Equals(Match match)
        {
            // If parameter is null return false:
            if ((object)match == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Host_TeamId == match.Host_TeamId) && (Visitor_TeamId == match.Visitor_TeamId);
        }

        //public override int GetHashCode()
        //{
        //    return Host.Initials ^ Visitor.Initials;
        //}
    }
}
