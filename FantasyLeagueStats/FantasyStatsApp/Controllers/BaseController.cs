using FantasyStats.Data;
using FantasyStats.Model;
using FantasyStatsApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FantasyStatsApp.Controllers
{
    public class BaseController : Controller
    {
        protected IUowData Data { get; set; }

        public BaseController(IUowData data)
        {
            this.Data = data;
        }

        public BaseController()
            : this(new UowData())
        {
        }

        protected string GetFullPosition(Position position)
        {
            switch (position)
            {
                case Position.GKP:
                    return "Goalkeeper";
                case Position.DEF:
                    return "Defender";
                case Position.MID:
                    return "Midfielder";
                case Position.FWD:
                    return "Forward";
                default:
                    break;
            }

            return String.Empty;
        }

        protected IQueryable<MatchViewModel> PopulateCurrentMatches()
        {
            DateTime currentDate = DateTime.Now;
            var currentGameweek = this.Data.Gameweeks.All()
                .FirstOrDefault(g => g.StartDate <= currentDate && currentDate <= g.EndDate);

            int currentGameweekId = currentGameweek == null ? 38 : currentGameweek.Id;

            var currentMatches = this.Data.Matches.All().Where(m => m.Gameweek.Id == currentGameweekId)
                .OrderBy(d => d.MatchDate).Select(MatchViewModel.FromMatches);

            return currentMatches;
        }

	}
}