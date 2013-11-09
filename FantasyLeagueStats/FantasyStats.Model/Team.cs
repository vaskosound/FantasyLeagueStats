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
