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
            string stats)
        {
            string[] playersStats = stats.Substring(1)
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var context = new ApplicationDbContext();
            foreach (var player in playersStats)
            {
                string[] playerInfo = player.Split(' ');
                int index = playerInfo.Length - 1;
                var playerModel = new PlayerModel()
                {
                    Selected = double.Parse(playerInfo[index - 4].TrimEnd('%')),
                    Points = int.Parse(playerInfo[index - 1]),
                    Team = playerInfo[index - 6],
                    MinutesPlayed = int.Parse(playerInfo[index].TrimEnd('\r'))
                };
                playerModel.SetName(playerInfo, index - 6);
                playerModel.SetPosition(playerInfo[index - 5]);
                playerModel.SetPrice(playerInfo[index - 3]);

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
            string statsPointsPerGame)
        {
            string[] playersStats = statsPointsPerGame.Substring(1)
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var context = new ApplicationDbContext();
            foreach (var player in playersStats)
            {
                string[] playerInfo = player.Split(' ');
                int index = playerInfo.Length - 1;
                var playerModel = new PlayerModel();                
                playerModel.SetName(playerInfo, index - 6);
                playerModel.PointsPerGame = decimal.Parse(playerInfo[index]);

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