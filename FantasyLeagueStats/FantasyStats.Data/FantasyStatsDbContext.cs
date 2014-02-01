using FantasyStats.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;

namespace FantasyStats.Data
{
    public class FantasyStatsDbContext : IdentityDbContext<ApplicationUser>
    {
        public FantasyStatsDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Match> Matches { get; set; }

        public DbSet<Gameweek> Gameweeks { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<PlayersGame> PlayerGames { get; set; }

        public DbSet<PlayersGamesGameweek> PlayersGamesGameweeks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PlayersGame>().HasKey(x => new { x.GameId, x.PlayerId, x.GamePlayer });
            modelBuilder.Entity<PlayersGamesGameweek>().HasKey(x => 
                new {x.GameweekId, x.GameId, x.PlayerId, x.GamePlayer });
        }
    }
}
