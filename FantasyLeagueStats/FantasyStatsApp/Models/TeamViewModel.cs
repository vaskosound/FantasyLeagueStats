using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class TeamViewModel
    {
        public static Expression<Func<Team, TeamViewModel>> FromTeams =
           team => new TeamViewModel()
           {
               Id = team.Id,
               Name = team.Name,
               GamesPlayed = team.GamesPlayed,
               Wins = team.Wins,
               Draws = team.Draws,
               Losses = team.Losses,
               Position = team.Position,
               Points = team.Points,
               GoalsFor = team.GoalsFor,
               GoalsAgainst = team.GoalsAgainst,
               GoalDifference = team.GoalsFor - team.GoalsAgainst
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