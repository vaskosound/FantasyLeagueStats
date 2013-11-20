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
    }
}
