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
        private static Dictionary<int, List<PlayerGameViewModel>> myTeam;
        private static SortedDictionary<int, List<PlayerGameViewModel>> pickTeamPlayers;
        private static int? currentGameweekId;
        private const int SUBSTITUTION = 5;

        public ActionResult JoinGame(int id)
        {
            var game = this.Data.Games.GetById(id);

            if (game.GameState != GameState.Created)
            {
                return RedirectToAction("JoinGames", "Games");
            }

            var userId = this.User.Identity.GetUserId();
            game.Second_PlayerId = userId;
            game.GameState = GameState.Full;
            this.Data.SaveChanges();

            return RedirectToAction("Transfers");
        }

        public ActionResult PickTeam(int id)
        {            
            myTeam = PopulateMyPlayers(id);
            if (myTeam == null || myTeam.Values.Sum(p => p.Count) < 15)
            {
                return RedirectToAction("Transfers", new { id = id });
            }

            var game = this.Data.Games.GetById(id);
            var playersInGame = this.Data.PlayersGames.All().Where(p => p.GameId == game.Id).ToList();

            currentGameweekId = this.Data.Gameweeks.All().FirstOrDefault(g => DateTime.Now <= g.Deadline).Id;
            var gamePlayersInGameweek = this.Data.PlayersGamesGameweeks.All()
                .Where(g => g.GameweekId == currentGameweekId && g.PlayersGame.GameId == game.Id)
                    .Select(x => x.PlayersGame).ToList();
            foreach (var playerInGame in playersInGame)
            {

                if (!gamePlayersInGameweek.Contains(playerInGame))
                {
                    PlayersGamesGameweek newPlayerInGameweek = new PlayersGamesGameweek()
                    {
                        GameweekId = currentGameweekId.Value,
                        PlayersGame = playerInGame,
                        IsStarting = playerInGame.IsStarting
                    };

                    this.Data.PlayersGamesGameweeks.Add(newPlayerInGameweek);
                }
            }

            this.Data.SaveChanges();

            ViewData["Fixtures"] = PopulateCurrentMatches();
            pickTeamPlayers = GetStartingPlayers(myTeam);

            return View(pickTeamPlayers);
        }

        public ActionResult StartingTeam(int id, List<PlayerGameViewModel> playersModel)
        {
            List<PlayerGameViewModel> startingPlayers = new List<PlayerGameViewModel>();
            foreach (var player in playersModel)
            {
                if (player.IsStarting)
                {
                    startingPlayers.Add(player);
                }
            }

            if (startingPlayers.Count != 11)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "The numer of starting players must be 11";
                return PartialView("_PickTeamPitch", pickTeamPlayers);
            }

            if (CountPlayersAtPosition(Position.GKP, startingPlayers) != 1)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "You can have only 1 goalkeeper";
                return PartialView("_PickTeamPitch", pickTeamPlayers);
            }

            if (CountPlayersAtPosition(Position.DEF, startingPlayers) < 3)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "The nuber of " + GetFullPosition(Position.DEF) + "s must be at least 3";
                return PartialView("_PickTeamPitch", pickTeamPlayers);
            }

            if (CountPlayersAtPosition(Position.MID, startingPlayers) < 3)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "The nuber of " + GetFullPosition(Position.MID) + "s must be at least 3";
                return PartialView("_PickTeamPitch", pickTeamPlayers);
            }

            if (CountPlayersAtPosition(Position.FWD, startingPlayers) < 1)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "The nuber of " + GetFullPosition(Position.FWD) + "s must be at least 1";
                return PartialView("_PickTeamPitch", pickTeamPlayers);
            }

            var playersInGame = this.Data.PlayersGames.All().Where(x => x.GameId == id).ToList();
            var gamePlayersInGameweek = this.Data.PlayersGamesGameweeks.All()
               .Where(g => g.GameweekId == currentGameweekId && g.PlayersGame.GameId == id)
                   .Select(x => x.PlayersGame).ToList();

            foreach (var player in playersModel)
            {
                var playerInGame = playersInGame.Find(p => p.PlayerId == player.Id);
                if (player.IsStarting != playerInGame.IsStarting)
                {
                    playerInGame.IsStarting = player.IsStarting;
                    var playerInGameweek = this.Data.PlayersGamesGameweeks.All()
                        .FirstOrDefault(g => g.PlayersGame.GameId == playerInGame.GameId &&
                            g.PlayersGame.PlayerId == playerInGame.PlayerId && g.GameweekId == currentGameweekId);
                    playerInGameweek.IsStarting = player.IsStarting;
                }
            }

            this.Data.SaveChanges();

            return PartialView("_PickTeamPitch", pickTeamPlayers);
        }

        public ActionResult OutPlayer(int playerId)
        {
            var player = myTeam.Values.FirstOrDefault(x => x.Any(p => p.Id == playerId))
                .FirstOrDefault(y => y.Id == playerId);
            player.IsStarting = false;

            pickTeamPlayers = GetStartingPlayers(myTeam);

            return PartialView("_PickTeamPitch", pickTeamPlayers);
        }

        public ActionResult InPlayer(int playerId)
        {
            var player = myTeam.Values.FirstOrDefault(x => x.Any(p => p.Id == playerId))
                .FirstOrDefault(y => y.Id == playerId);
            player.IsStarting = true;

            pickTeamPlayers = GetStartingPlayers(myTeam);

            return PartialView("_PickTeamPitch", pickTeamPlayers);
        }

        public ActionResult Transfers(int id)
        {
            var game = this.Data.Games.GetById(id);
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
            var game = this.Data.Games.GetById(id);
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

            if (selectPlayers == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "Select any player";
                return View("_TransferPitch", myPlayers);
            }

            var playerExists = this.Data.Players.GetById((int)selectPlayers);

            if (budget - playerExists.Price < 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "Not enough money";
                return View("_TransferPitch", myPlayers);
            }

            if (!IsValidPosition(playerExists))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "You already have tha maximum number of " +
                    GetFullPosition(playerExists.Position) + "s";
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
            var playerInGameweek = new PlayersGamesGameweek()
            {
                GameweekId = currentGameweekId.Value,
                PlayersGame = playerInGame
            };

            this.Data.PlayersGamesGameweeks.Add(playerInGameweek);

            if (gamePlayer == GamePlayer.FirstPlayer)
            {
                game.FirstUserBudget = budget;
            }
            else
            {
                game.SecondUserBudget = budget;
            }


            this.Data.SaveChanges();

            var playersInGameCount = this.Data.PlayersGames.All()
                .Where(g => g.GameId == id).Count();
            if (game.GameState != GameState.InProgress && playersInGameCount == 30)
            {
                game.GameState = GameState.InProgress;
                game.StartedGameweek = this.Data.Gameweeks.All()
                    .FirstOrDefault(g => g.StartDate < DateTime.Now && DateTime.Now < g.Deadline).Id;
                this.Data.SaveChanges();
            }

            ViewBag.Budget = budget;
            myTeam = PopulateMyPlayers(id);
            myPlayers = MyPlayersInArray(myTeam);

            return PartialView("_TransferPitch", myPlayers);
        }

        public ActionResult RemovePlayer(int id, int playerId)
        {
            var game = this.Data.Games.GetById(id);
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
            var currentGameweek = this.Data.Gameweeks.All()
                .FirstOrDefault(g => DateTime.Now <= g.Deadline);
            var playerInGameweek = this.Data.PlayersGamesGameweeks.All()
                .FirstOrDefault(g => g.PlayersGame.GameId == id &&
                    g.PlayersGame.PlayerId == playerId && g.GameweekId == currentGameweekId);
            this.Data.PlayersGamesGameweeks.Delete(playerInGameweek);
            this.Data.SaveChanges();

            myTeam = PopulateMyPlayers(id);
            var myPlayers = MyPlayersInArray(myTeam);

            return PartialView("_TransferPitch", myPlayers);
        }

        public ActionResult OpponentTeam(int id)
        {
            var game = this.Data.Games.GetById(id);
            string userId = this.User.Identity.GetUserId();
            GamePlayer gamePlayer;
            decimal opponentBudget = 0;
            if (game.First_PlayerId != userId)
            {
                opponentBudget = game.FirstUserBudget;
                gamePlayer = GamePlayer.FirstPlayer;
            }
            else
            {
                opponentBudget = game.SecondUserBudget;
                gamePlayer = GamePlayer.SecondPlayer;
            }

            ViewBag.Budget = opponentBudget;
            ViewData["Fixtures"] = PopulateCurrentMatches();
            var opponentPlayers = this.Data.PlayersGames.All().Where(p => p.GameId == id &&
                 p.GamePlayer == gamePlayer).Select(PlayerGameViewModel.FromPlayersGame).ToList();

            Dictionary<int, List<PlayerGameViewModel>> players = PlayersInDictionary(opponentPlayers);

            return View(GetStartingPlayers(players));
        }

        private bool IsStarting(Player playerExists)
        {
            switch (playerExists.Position)
            {
                case Position.GKP:
                    if (myTeam.ContainsKey((int)Position.GKP) &&
                        myTeam[(int)Position.GKP].Count == 1 && myTeam[(int)Position.GKP][0].IsStarting)
                    {
                        return false;
                    }
                    break;
                case Position.DEF:
                    if (myTeam.ContainsKey((int)Position.DEF) && myTeam[(int)Position.DEF].Count > 3)
                    {
                        int count = 0;
                        foreach (var player in myTeam[(int)Position.DEF])
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
                    if (myTeam.ContainsKey((int)Position.MID) && myTeam[(int)Position.MID].Count > 3)
                    {
                        int count = 0;
                        foreach (var player in myTeam[(int)Position.MID])
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
                    if (myTeam.ContainsKey((int)Position.FWD) && myTeam[(int)Position.FWD].Count > 1)
                    {
                        int count = 0;
                        foreach (var player in myTeam[(int)Position.FWD])
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
                    if (myTeam.ContainsKey((int)Position.GKP) && myTeam[(int)Position.GKP].Count == 2)
                    {
                        return false;
                    }
                    break;
                case Position.DEF:
                    if (myTeam.ContainsKey((int)Position.DEF) && myTeam[(int)Position.DEF].Count == 5)
                    {
                        return false;
                    }
                    break;
                case Position.MID:
                    if (myTeam.ContainsKey((int)Position.MID) && myTeam[(int)Position.MID].Count == 5)
                    {
                        return false;
                    }
                    break;
                case Position.FWD:
                    if (myTeam.ContainsKey((int)Position.FWD) && myTeam[(int)Position.FWD].Count == 3)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        private int CountPlayersAtPosition(Position position, List<PlayerGameViewModel> players)
        {
            int counter = 0;
            foreach (var player in players)
            {
                if (player.Position == position)
                {
                    counter++;
                }
            }

            return counter;
        }

        private SortedDictionary<int, List<PlayerGameViewModel>> GetStartingPlayers(
            Dictionary<int, List<PlayerGameViewModel>> team)
        {
            SortedDictionary<int, List<PlayerGameViewModel>> players =
                new SortedDictionary<int, List<PlayerGameViewModel>>();

            foreach (var position in team)
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
                        if (!players.ContainsKey(SUBSTITUTION))
                        {
                            players.Add(SUBSTITUTION, new List<PlayerGameViewModel>());
                        }

                        players[SUBSTITUTION].Add(player);
                        players[SUBSTITUTION].OrderBy(x => (int)x.Position);
                    }
                }
            }
            return players;
        }

        public ActionResult GameweekResult(int id)
        {
            var game = this.Data.Games.GetById(id);
            currentGameweekId = this.Data.Gameweeks.All()
                .FirstOrDefault(g => DateTime.Now <= g.Deadline).Id;
            int previousGameweekId = currentGameweekId.Value - 1;
            if (currentGameweekId == 1)
            {
                previousGameweekId = currentGameweekId.Value;
            }

            if (currentGameweekId == null)
            {
                previousGameweekId = 38;
            }
            GameResultsViewModel results = new GameResultsViewModel();
            if (game.GameState != GameState.InProgress)
            {
                return View(results);
            }

            var playersInGameweek = this.Data.PlayersGamesGameweeks.All()
                .Where(p => p.PlayersGame.GameId == id && p.GameweekId == currentGameweekId);
            var playersList = playersInGameweek.Select(g => g.PlayersGame.Player).ToList();

            var playersInGameweekToList = playersInGameweek.ToList();
            for (int i = 0; i < playersInGameweekToList.Count; i++)
            {
                var currentPlayer = playersList.FirstOrDefault(p => p.Id ==
                    playersInGameweekToList[i].PlayersGame.PlayerId);
                playersInGameweekToList[i].Points = currentPlayer.RoundScore;
            }

            this.Data.SaveChanges();

            var firstUserPlayersList = playersInGameweek.Where(u => u.PlayersGame.GamePlayer ==
                GamePlayer.FirstPlayer).Select(PlayerGameViewModel.FromPlayersGameweek).ToList();
            var secondUserPlayersList = playersInGameweek.Where(u => u.PlayersGame.GamePlayer ==
                GamePlayer.SecondPlayer).Select(PlayerGameViewModel.FromPlayersGameweek).ToList();

            Dictionary<int, List<PlayerGameViewModel>> firstUserPlayers =
                PlayersInDictionary(firstUserPlayersList);
            var firstUserStartingPlayers = GetStartingPlayers(firstUserPlayers);

            Dictionary<int, List<PlayerGameViewModel>> secondUserPlayers =
                PlayersInDictionary(secondUserPlayersList);
            var secondUserStartingPlayers = GetStartingPlayers(secondUserPlayers);

            game.FirstUserGWPoints = CalculateTotalPoints(firstUserPlayersList);
            game.SecondUserGWPoints = CalculateTotalPoints(secondUserPlayersList);

            var firstUserPlayersInGameweeks = this.Data.PlayersGamesGameweeks.All()
                .Where(g => g.PlayersGame.GameId == game.Id && g.IsStarting &&
                    g.PlayersGame.GamePlayer == GamePlayer.FirstPlayer)
                    .Select(PlayerGameViewModel.FromPlayersGameweek).ToList();
            var secondUserPlayersInGameweeks = this.Data.PlayersGamesGameweeks.All()
                .Where(g => g.PlayersGame.GameId == game.Id && g.IsStarting &&
                    g.PlayersGame.GamePlayer == GamePlayer.SecondPlayer)
                        .Select(PlayerGameViewModel.FromPlayersGameweek).ToList();
            game.FirstUserPoints = CalculateTotalPoints(firstUserPlayersInGameweeks);
            game.SecondUserPoints = CalculateTotalPoints(secondUserPlayersInGameweeks);
            this.Data.SaveChanges();
            ViewBag.FirstPlayerPoints = game.FirstUserGWPoints;
            ViewBag.SecondPlayerPoints = game.SecondUserGWPoints;

            results.FirstUserPlayers = firstUserStartingPlayers;
            results.SecondUserPlayers = secondUserStartingPlayers;

            return View(results);
        }

        private int CalculateTotalPoints(List<PlayerGameViewModel> teamPlayers)
        {
            int totalPoints = 0;
            foreach (var player in teamPlayers)
            {
                if (player.IsStarting)
                {
                    totalPoints += player.RoundScore;
                }
            }

            return totalPoints;
        }

        private Dictionary<int, List<PlayerGameViewModel>> PopulateMyPlayers(int id)
        {
            var game = this.Data.Games.GetById(id);
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

            Dictionary<int, List<PlayerGameViewModel>> players = PlayersInDictionary(myPlayers);

            return players;
        }

        private static Dictionary<int, List<PlayerGameViewModel>> PlayersInDictionary(
            List<PlayerGameViewModel> players)
        {
            Dictionary<int, List<PlayerGameViewModel>> playersInDictionary =
                new Dictionary<int, List<PlayerGameViewModel>>();

            foreach (var player in players)
            {
                if (!playersInDictionary.ContainsKey((int)player.Position))
                {
                    playersInDictionary.Add((int)player.Position, new List<PlayerGameViewModel>() { player });
                }
                else
                {
                    playersInDictionary[(int)player.Position].Add(player);
                }
            }
            return playersInDictionary;
        }

        private PlayerGameViewModel[] MyPlayersInArray(
            Dictionary<int, List<PlayerGameViewModel>> players)
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
    }
}