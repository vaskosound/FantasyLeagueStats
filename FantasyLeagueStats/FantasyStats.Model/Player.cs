using System;
using System.Linq;

namespace FantasyStats.Model
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TeamId { get; set; }

        public virtual Team Team { get; set; }

        public Position Position { get; set; }

        public double Selected { get; set; }

        public int Points { get; set; }

        public decimal Price { get; set; }

        public int MinutesPlayed { get; set; }

        public decimal PointsPerGame { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} -", this.Name, this.Team.Initials, this.Position.ToString());
        }
    }
}

