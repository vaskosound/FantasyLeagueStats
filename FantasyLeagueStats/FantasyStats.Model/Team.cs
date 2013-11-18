using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FantasyStats.Model
{
    public class Team
    {
        private HashSet<Player> players;

        public int Id { get; set; }

        public string Name { get; set; }

        public string Initials { get; set; }

        public int Position { get; set; }

        public int GamesPlayed { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }

        public int Points { get; set; }
    
        public Team()
        {
            this.players = new HashSet<Player>();
        }

        public HashSet<Player> Players
        {
            get { return this.players; }
            set { this.players = value; }
        }
    }
}
