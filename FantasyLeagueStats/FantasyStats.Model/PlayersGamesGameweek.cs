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
        [Key]
        public int Id { get; set; }

        public int GameweekId { get; set; }

        [Required]
        public bool IsStarting { get; set; }

        public virtual PlayersGame PlayersGame { get; set; }

        public virtual Gameweek Gameweek { get; set; }

        public int Points { get; set; }

        public PlayersGamesGameweek()
        {
            IsStarting = true;
        }
    }
}
