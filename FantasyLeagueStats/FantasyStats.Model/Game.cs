using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyStats.Model
{
    public class Game
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string First_PlayerId { get; set; }

        [ForeignKey("First_PlayerId"), Column(Order = 0)]
        public virtual ApplicationUser FirstUser { get; set; }

        public string Second_PlayerId { get; set; }

        [ForeignKey("Second_PlayerId"), Column(Order = 1)]
        public virtual ApplicationUser SecondUser { get; set; }

        public ICollection<Player> Players { get; set; }

        public int FirstUserPoints { get; set; }

        public int SecondUserPoints { get; set; }

        public int FirstUserGWPoints { get; set; }

        public int SecondUserGWPoints { get; set; }

        public GameState GameState { get; set; }
        public Game()
        {
            this.Players = new HashSet<Player>();
        }
    }
}
