using FantasyStats.Model;
using System;
using System.Linq;

namespace FantasyStats.Data
{
    public interface IUowData
    {
        IRepository<Player> Players { get; }

        IRepository<Match> Matches { get; }

        IRepository<Team> Teams { get; }

        IRepository<Gameweek> Gameweeks { get; }

        IRepository<ApplicationUser> Users { get; }

        IRepository<Game> Games { get; }

        int SaveChanges();
    }
}
