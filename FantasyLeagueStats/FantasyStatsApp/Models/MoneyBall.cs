using FantasyStats.Data;
using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class MoneyBall
    {
        public static void UpdateBasicData(
            List<string> stats)
        {
            var context = new ApplicationDbContext();
            for (int i = 0; i < stats.Count; i += 8)
            {
                var playerModel = new PlayerModel()
                {
                    Name = stats[i],
                    Selected = double.Parse(stats[i + 3].TrimEnd('%')),
                    Points = int.Parse(stats[i + 6]),
                    Team = stats[i + 1],
                    MinutesPlayed = int.Parse(stats[i + 7])
                };
                playerModel.SetPosition(stats[i + 2]);
                playerModel.SetPrice(stats[i + 4]);

                var playerExists = context.Players.FirstOrDefault(x => x.Name == playerModel.Name);
                var team = context.Teams.FirstOrDefault(x => x.Initials == playerModel.Team);
                if (playerExists == null)
                {
                    Player newPlayer = new Player()
                    {
                        Name = playerModel.Name,
                        Position = playerModel.Position,
                        Selected = playerModel.Selected,
                        Price = playerModel.Price,
                        Points = playerModel.Points,
                        MinutesPlayed = playerModel.MinutesPlayed
                    };
                    team.Players.Add(newPlayer);
                }
                else
                {
                    playerExists.TeamId = team.Id;
                    playerExists.Selected = playerModel.Selected;
                    playerExists.Points = playerModel.Points;
                    playerExists.Price = playerModel.Price;
                    playerExists.MinutesPlayed = playerModel.MinutesPlayed;
                }
                context.SaveChanges();
            }
        }

        public static void UpdatePointsPerGameData(
            List<string> stats)
        {
            var context = new ApplicationDbContext();
            for (int i = 0; i < stats.Count; i += 8)
            {
                var playerModel = new PlayerModel() 
                {
                    Name = stats[i],
                    PointsPerGame = decimal.Parse(stats[i + 7])
                };   

                var playerExists = context.Players.FirstOrDefault(x => x.Name == playerModel.Name);
                if (playerExists != null)
                {
                    playerExists.PointsPerGame = playerModel.PointsPerGame;
                }                
            }

            context.SaveChanges();         
        }
    }
}