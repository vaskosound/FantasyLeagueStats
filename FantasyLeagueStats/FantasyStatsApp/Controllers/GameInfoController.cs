using FantasyStats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FantasyStatsApp.Models.GameModels;
using FantasyStatsApp.Models;
using System.Net;

namespace FantasyStatsApp.Controllers
{
    public class GameInfoController : BaseController
    {
        private static Dictionary<Position, List<PlayerGameViewModel>> myTeam;
        private static Game game;
        private static SortedDictionary<Position, List<PlayerGameViewModel>> pickTeamPlayers;
        public ActionResult PickTeam(int id)
        {
            game = this.Data.Games.GetById(id);

            myTeam = PopulateMyPlayers(id);
            if (myTeam == null || myTeam.Values.Sum(p => p.Count) < 15)
            {
                return RedirectToAction("Transfers", new { id = id });
            }

            pickTeamPlayers = GetStartingPlayers();

            return View(pickTeamPlayers);
        }

        private static SortedDictionary<Position, List<PlayerGameViewModel>> GetStartingPlayers()
        {
            SortedDictionary<Position, List<PlayerGameViewModel>> players =
                new SortedDictionary<Position, List<PlayerGameViewModel>>();

            foreach (var position in myTeam)
            {
                foreach (var player in position.Value)
                {
                    if (!players.ContainsKey(position.Key))
                    {
                        players.Add(position.Key, new List<PlayerGameViewModel>());
                    }

                    if (player.IsStarting)
                    {
                        players[position.Key].Add(player);
                    }
                    else
                    {
                        if (!players.ContainsKey(Position.SUB))
                        {
                            players.Add(Position.SUB, new List<PlayerGameViewModel>());
                        }

                        players[Position.SUB].Add(player);
                       // players[Position.SUB].OrderBy(x => (int)x.Position);
                    }
                }
            }
            return players;
        }

        public ActionResult OutPlayer(int playerId)
        {
            var player = myTeam.Values.FirstOrDefault(x => x.Any(p => p.Id == playerId))
                .FirstOrDefault(y => y.Id == playerId);
            player.IsStarting = false;

            pickTeamPlayers = GetStartingPlayers();

            return PartialView("_PickTeamPitch", pickTeamPlayers);
        }

        public ActionResult InPlayer(int playerId)
        {
            var player = myTeam.Values.FirstOrDefault(x => x.Any(p => p.Id == playerId))
                .FirstOrDefault(y => y.Id == playerId);
            player.IsStarting = true;

            pickTeamPlayers = GetStartingPlayers();

            return PartialView("_PickTeamPitch", pickTeamPlayers);
        }
                
        public ActionResult Transfers(int id)
        {
            myTeam = PopulateMyPlayers(id);

            decimal playerBudget;
            if (game.First_PlayerId == User.Identity.GetUserId())
            {
                playerBudget = game.FirstUserBudget;
            }
            else
            {
                playerBudget = game.SecondUserBudget;
            }

            var myPlayersArray = MyPlayersInArray(myTeam);

            decimal playersPrice = 0;
            foreach (var player in myPlayersArray)
            {
                if (player != null)
                {
                    playersPrice += player.Price;
                }
            }

            TransferViewModel transferViewModel = new TransferViewModel()
            {
                Team = myPlayersArray,
                Players = this.Data.Players.All().Select(PlayerBasicModel.FromPlayersStats)
            };

            ViewBag.Budget = playerBudget;

            return View(transferViewModel);
        }

        public JsonResult GetClubs()
        {
            var clubs = this.Data.Teams.All().Select(SelectTeamModel.FromTeams);

            return Json(clubs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlayers(int id, int? teams)
        {
            var gamePlayers = this.Data.PlayersGames.All().Where(x => x.GameId == id).Select(p => p.Player);

            var players = this.Data.Players.All().Where(x => !gamePlayers.Contains(x));

            if (teams != null)
            {
                players = players.Where(p => p.TeamId == teams);
            }

            return Json(players.Select(SelectPlayerModel.FromPlayers), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddPlayer(int id, int? selectPlayers)
        {
            var myPlayers = MyPlayersInArray(myTeam);
            GamePlayer gamePlayer;
            decimal budget = 0;
            if (game.First_PlayerId == User.Identity.GetUserId())
            {
                gamePlayer = GamePlayer.FirstPlayer;
                budget = game.FirstUserBudget;
            }
            else
            {
                gamePlayer = GamePlayer.SecondPlayer;
                budget = game.SecondUserBudget;
            }

            if (selectPlayers != null)
            {
                var playerExists = this.Data.Players.GetById((int)selectPlayers);

                if (budget - playerExists.Price < 0 || !IsValidPosition(playerExists))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return View("_TransferPitch", myPlayers);
                }

                budget -= playerExists.Price;

                var playerInGame = new PlayersGame()
                {
                    GameId = id,
                    PlayerId = playerExists.Id,
                    GamePlayer = gamePlayer
                };

                int startingPlayers = myTeam.Values.Select(x => x.Where(y => y.IsStarting))
                    .Sum(p => p.Count());

                if (startingPlayers == 11 || !IsStarting(playerExists))
                {
                    playerInGame.IsStarting = false;
                }

                this.Data.PlayersGames.Add(playerInGame);

                if (gamePlayer == GamePlayer.FirstPlayer)
                {
                    game.FirstUserBudget = budget;
                }
                else
                {
                    game.SecondUserBudget = budget;
                }

                this.Data.SaveChanges();
            }

            ViewBag.Budget = budget;
            myTeam = PopulateMyPlayers(id);
            myPlayers = MyPlayersInArray(myTeam);

            return PartialView("_TransferPitch", myPlayers);
        }

        private bool IsStarting(Player playerExists)
        {
            switch (playerExists.Position)
            {
                case Position.GKP:
                    if (myTeam.ContainsKey(Position.GKP) &&
                        myTeam[Position.GKP].Count == 1 && myTeam[Position.GKP][0].IsStarting)
                    {
                        return false;
                    }
                    break;
                case Position.DEF:
                    if (myTeam.ContainsKey(Position.DEF) && myTeam[Position.DEF].Count > 3)
                    {
                        int count = 0;
                        foreach (var player in myTeam[Position.DEF])
                        {
                            if (player.IsStarting)
                            {
                                count++;
                            }

                            if (count > 3)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case Position.MID:
                    if (myTeam.ContainsKey(Position.MID) && myTeam[Position.MID].Count > 3)
                    {
                        int count = 0;
                        foreach (var player in myTeam[Position.MID])
                        {
                            if (player.IsStarting)
                            {
                                count++;
                            }

                            if (count > 3)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case Position.FWD:
                    if (myTeam.ContainsKey(Position.FWD) && myTeam[Position.FWD].Count > 1)
                    {
                        int count = 0;
                        foreach (var player in myTeam[Position.FWD])
                        {
                            if (player.IsStarting)
                            {
                                count++;
                            }

                            if (count > 1)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        private bool IsValidPosition(Player playerExists)
        {
            switch (playerExists.Position)
            {
                case Position.GKP:
                    if (myTeam.ContainsKey(Position.GKP) && myTeam[Position.GKP].Count == 2)
                    {
                        return false;
                    }
                    break;
                case Position.DEF:
                    if (myTeam.ContainsKey(Position.DEF) && myTeam[Position.DEF].Count == 5)
                    {
                        return false;
                    }
                    break;
                case Position.MID:
                    if (myTeam.ContainsKey(Position.MID) && myTeam[Position.MID].Count == 5)
                    {
                        return false;
                    }
                    break;
                case Position.FWD:
                    if (myTeam.ContainsKey(Position.FWD) && myTeam[Position.FWD].Count == 3)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        public ActionResult GameweekResult(int id)
        {
            var players = this.Data.PlayersGames.All().Where(p => p.GameId == id)
               .Select(x => x.Player).Select(p => PlayerBasicModel.FromPlayersStats).ToList();
            return View(players);
        }

        public ActionResult RemovePlayer(int id, int playerId)
        {
            decimal playerPrice = this.Data.Players.GetById(playerId).Price;
            decimal budget = 0;
            if (game.First_PlayerId == User.Identity.GetUserId())
            {
                game.FirstUserBudget += playerPrice;
                budget = game.FirstUserBudget;
            }
            else
            {
                game.SecondUserBudget += playerPrice;
                budget = game.SecondUserBudget;
            }

            ViewBag.Budget = budget;

            var playerInGame = this.Data.PlayersGames.All()
                .FirstOrDefault(p => p.GameId == id && p.PlayerId == playerId);
            this.Data.PlayersGames.Delete(playerInGame);
            this.Data.SaveChanges();

            myTeam = PopulateMyPlayers(id);
            var myPlayers = MyPlayersInArray(myTeam);

            return PartialView("_TransferPitch", myPlayers);
        }

        private Dictionary<Position, List<PlayerGameViewModel>> PopulateMyPlayers(int id)
        {
            GamePlayer gamePlayer;
            if (game.First_PlayerId == User.Identity.GetUserId())
            {
                gamePlayer = GamePlayer.FirstPlayer;
            }
            else
            {
                gamePlayer = GamePlayer.SecondPlayer;
            }

            var myPlayers = this.Data.PlayersGames.All().Where(p => p.GameId == id &&
                p.GamePlayer == gamePlayer).Select(PlayerGameViewModel.FromPlayersGame).ToList();

            Dictionary<Position, List<PlayerGameViewModel>> players =
                new Dictionary<Position, List<PlayerGameViewModel>>();

            foreach (var player in myPlayers)
            {
                if (!players.ContainsKey(player.Position))
                {
                    players.Add(player.Position, new List<PlayerGameViewModel>() { player });
                }
                else
                {
                    players[player.Position].Add(player);
                }
            }

            return players;
        }

        private PlayerGameViewModel[] MyPlayersInArray(
            Dictionary<Position, List<PlayerGameViewModel>> players)
        {
            PlayerGameViewModel[] playersArr = new PlayerGameViewModel[15];
            foreach (var position in players)
            {
                foreach (var player in position.Value)
                {
                    AddPlayerInArray(playersArr, player);
                }
            }

            return playersArr;
        }

        private void AddPlayerInArray(PlayerGameViewModel[] playersArr, PlayerGameViewModel player)
        {
            switch (player.Position)
            {
                case Position.GKP:
                    for (int i = 0; i < 2; i++)
                    {
                        if (playersArr[i] == null)
                        {
                            playersArr[i] = player;
                            break;
                        }
                    }
                    break;
                case Position.DEF:
                    for (int i = 2; i < 7; i++)
                    {
                        if (playersArr[i] == null)
                        {
                            playersArr[i] = player;
                            break;
                        }
                    }
                    break;
                case Position.MID:
                    for (int i = 7; i < 12; i++)
                    {
                        if (playersArr[i] == null)
                        {
                            playersArr[i] = player;
                            break;
                        }
                    }
                    break;
                case Position.FWD:
                    for (int i = 12; i < 15; i++)
                    {
                        if (playersArr[i] == null)
                        {
                            playersArr[i] = player;
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void AddStartingPlayerInArray(PlayerGameViewModel[] playersArr, PlayerGameViewModel player)
        {
            throw new NotImplementedException();
        }

    }
}