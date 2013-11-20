using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FantasyStats.Model
{
    public class Gameweek
    {
        private HashSet<Match> matches;

        [KeyAttribute()]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Gameweek()
        {
            this.matches = new HashSet<Match>();
        }

        public HashSet<Match> Matches
        {
            get { return this.matches; }
            set { this.matches = value; }
        }
    }
}
