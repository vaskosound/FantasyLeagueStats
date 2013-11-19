using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyStats.Data
{
    public interface IUowData
    {
        IRepository<Player> Players { get; }

        IRepository<Match> Matches { get; }

        IRepository<Team> Teams { get; }

        IRepository<Gameweek> Gameweeks { get; }

        IRepository<ApplicationUser> Users { get; }

        int SaveChanges();
    }
}
