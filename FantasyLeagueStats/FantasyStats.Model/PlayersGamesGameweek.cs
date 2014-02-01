using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyStats.Model
{
    public class PlayersGamesGameweek
    {
        [Key, ForeignKey("Gameweek")]
        public int GameweekId { get; set; }

        public virtual Gameweek Gameweek { get; set; }
        
        [Key, ForeignKey("Player")]
        public int PlayerId { get; set; }

        [Key, ForeignKey("Game")]
        public int GameId { get; set; }

        [Required]
        public GamePlayer GamePlayer { get; set; }

        [Required]
        public bool IsStarting { get; set; }

        public virtual Player Player { get; set; }

        public virtual Game Game { get; set; }

        public int Points { get; set; }

        public string TeamAgainst { get; set; }

        public PlayersGamesGameweek()
        {
            IsStarting = true;
        }
    }
}
