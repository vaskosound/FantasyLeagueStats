using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyStats.Model
{
    public class PlayersGame
    {
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

        public PlayersGame()
        {
            this.IsStarting = true;
        }
    }
}
