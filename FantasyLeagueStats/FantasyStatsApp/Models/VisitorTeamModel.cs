using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class VisitorTeamModel
    {
        public static Expression<Func<Match, VisitorTeamModel>> FromMatch =
          match => new VisitorTeamModel()
          {
              Id = match.Visitor.Id,
              Name = match.Visitor.Name,
              GamesPlayed = match.Visitor.GamesPlayed,
              Wins = match.Visitor.Wins,
              Draws = match.Visitor.Draws,
              Losses = match.Visitor.Losses,
              Position = match.Visitor.Position,
              Points = match.Visitor.Points,
              GoalsFor = match.Visitor.GoalsFor,
              GoalsAgainst = match.Visitor.GoalsAgainst,
              GoalDifference = match.Visitor.GoalsFor - match.Visitor.GoalsAgainst
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