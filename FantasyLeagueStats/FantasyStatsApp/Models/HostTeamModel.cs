using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class HostTeamModel
    {
        public static Expression<Func<Match, HostTeamModel>> FromMatch =
          match => new HostTeamModel()
          {
              Id = match.Host.Id,
              Name = match.Host.Name,
              GamesPlayed = match.Host.GamesPlayed,
              Wins = match.Host.Wins,
              Draws = match.Host.Draws,
              Losses = match.Host.Losses,
              Position = match.Host.Position,
              Points = match.Host.Points,
              GoalsFor = match.Host.GoalsFor,
              GoalsAgainst = match.Host.GoalsAgainst,
              GoalDifference = match.Host.GoalsFor - match.Host.GoalsAgainst
          };
        public int Id { get; set; }

        public string Name { get; set; }

        public int Position { get; set; }

        public int GamesPlayed { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }

        public int GoalDifference { get; set; }

        public int Points { get; set; }
    }
}
