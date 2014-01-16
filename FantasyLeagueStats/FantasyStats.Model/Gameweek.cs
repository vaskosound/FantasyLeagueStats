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
       // private HashSet<PlayersGame> playersGames;

        [KeyAttribute()]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime Deadline { get; set; }

        public Gameweek()
        {
            this.matches = new HashSet<Match>();
         //   this.playersGames = new HashSet<PlayersGame>();
        }

        public virtual HashSet<Match> Matches
        {
            get { return this.matches; }
            set { this.matches = value; }
        }

        //public virtual HashSet<PlayersGame> PlayersGames
        //{
        //    get { return this.playersGames; }
        //    set { this.playersGames = value; }
        //}
    }
}
