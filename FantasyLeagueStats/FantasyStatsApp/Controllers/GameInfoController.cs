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
        private static decimal budget;
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
            currentGameweekId = this.Data.Gameweeks.All().FirstOrDefault(g => DateTime.Now <= g.Deadline).Id;
            myTeam = PopulateMyPlayers(id);
            if (myTeam == null || myTeam.Values.Sum(p => p.Count) < 15)
            {
                return RedirectToAction("Transfers", new { id = id });
            }

            var game = this.Data.Games.GetById(id);
            var playersInGame = this.Data.PlayersGames.All().Where(p => p.GameId == game.Id);

            var playersInGameId = playersInGame.Select(x => x.PlayerId).ToList();

            var gamePlayersInGameweekId = this.Data.PlayersGamesGameweeks.All()
                .Where(g => g.GameweekId == currentGameweekId && g.GameId == game.Id)
                    .Select(x => x.PlayerId).ToList();

            this.Data.PlayersGamesGameweeks.DeleteRange(p => !playersInGameId.Contains(p.PlayerId));
            foreach (var playerInGame in playersInGame)
            {
                if (!gamePlayersInGameweekId.Contains(playerInGame.PlayerId))
                {                    
                    string aginstTeam = GetTeamAgainstInGameweek(playerInGame.Player);

                    PlayersGamesGameweek newPlayerInGameweek = new PlayersGamesGameweek()
                    {
                        GameweekId = currentGameweekId.Value,
                        PlayerId = playerInGame.PlayerId,
                        IsStarting = playerInGame.IsStarting,
                        GameId = game.Id,
                        GamePlayer = playerInGame.GamePlayer,
                        TeamAgainst = aginstTeam
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
               .Where(g => g.GameweekId == currentGameweekId && g.GameId == id).ToList();

            foreach (var player in playersModel)
            {
                var playerInGame = playersInGame.Find(p => p.PlayerId == player.Id);
                if (player.IsStarting != playerInGame.IsStarting)
                {
                    playerInGame.IsStarting = player.IsStarting;
                    var playerInGameweek = this.Data.PlayersGamesGameweeks.All()
                        .FirstOrDefault(g => g.GameId == playerInGame.GameId &&
                            g.PlayerId == playerInGame.PlayerId && g.GameweekId == currentGameweekId);
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

            if (game.First_PlayerId == User.Identity.GetUserId())
            {
                budget = game.FirstUserBudget;
            }
            else
            {
                budget = game.SecondUserBudget;
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

            ViewBag.Budget = budget;

            return View(myPlayersArray);
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

        public ActionResult AddPlayer(int id, int? selectPlayerId)
        {
            var game = this.Data.Games.GetById(id);
            var myPlayers = MyPlayersInArray(myTeam);

            if (selectPlayerId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "Select any player";
                return View("_TransferPitch", myPlayers);
            }

            var playerExists = this.Data.Players.GetById(selectPlayerId.Value);

            if (!IsValidPosition(playerExists))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "You already have tha maximum number of " +
                    GetFullPosition(playerExists.Position) + "s";
                return View("_TransferPitch", myPlayers);
            }

            budget -= playerExists.Price;

            PlayerGameViewModel newPlayer = new PlayerGameViewModel()
            {
                Id = playerExists.Id,
                Name = playerExists.Name,
                Position = playerExists.Position,
                Price = playerExists.Price,
                Points = playerExists.Points,
                RoundScore = playerExists.RoundScore,
                Team = playerExists.Team.Initials,
                IsStarting = true
            };

            int startingPlayers = myTeam.Values.Select(x => x.Where(y => y.IsStarting))
                .Sum(p => p.Count());

            if (startingPlayers == 11 || !IsStarting(playerExists))
            {
                newPlayer.IsStarting = false;
            }


            ViewBag.Budget = budget;
            if (!myTeam.ContainsKey((int)newPlayer.Position))
            {
                myTeam.Add((int)newPlayer.Position, new List<PlayerGameViewModel>());
            }

            myTeam[(int)newPlayer.Position].Add(newPlayer);
            myPlayers = MyPlayersInArray(myTeam);

            return PartialView("_TransferPitch", myPlayers);
        }

        public ActionResult RemovePlayer(int id, int playerId)
        {
            var game = this.Data.Games.GetById(id);
            decimal playerPrice = this.Data.Players.GetById(playerId).Price;
            budget += playerPrice;
            ViewBag.Budget = budget;

            var playerToRemove = myTeam.Values.FirstOrDefault(p => p.Any(x => x.Id == playerId))
                .FirstOrDefault(y => y.Id == playerId);
            myTeam[(int)playerToRemove.Position].Remove(playerToRemove);
            var myPlayers = MyPlayersInArray(myTeam);

            return PartialView("_TransferPitch", myPlayers);
        }

        public ActionResult ConfirmTransfers(int id, PlayerGameViewModel[] playersModel)
        {
            var game = this.Data.Games.GetById(id);
            if (budget < 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = "Not enough money";
                return PartialView("_TransferPitch", playersModel);
            }

            int myPlayersCount = GetMyPlayersCount(playersModel);

            if (myPlayersCount < 15)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.StatusDescription = string.Format("You have only {0} players", myPlayersCount);
                return PartialView("_TransferPitch", playersModel);
            }

            GamePlayer gamePlayer;
            if (game.First_PlayerId == User.Identity.GetUserId())
            {
                gamePlayer = GamePlayer.FirstPlayer;
            }
            else
            {
                gamePlayer = GamePlayer.SecondPlayer;
            }

            List<int> playersModelId = new List<int>();
            List<Player> myPlayersToConfirm = new List<Player>();
            foreach (var player in playersModel)
            {
                var playerToConfirm = this.Data.Players.GetById(player.Id);
                myPlayersToConfirm.Add(playerToConfirm);
                playersModelId.Add(player.Id);
            }

            var myPlayersGame = this.Data.PlayersGames.All()
                .Where(g => g.GameId == game.Id && g.GamePlayer == gamePlayer);
            var myPlayersInGame = myPlayersGame.Select(p => p.Player).ToList();

            var gamePlayersGameweek = this.Data.PlayersGamesGameweeks.All()
                .Where(g => g.GameweekId == currentGameweekId && g.GameId == game.Id);
            var gamePlayersInGameweek = gamePlayersGameweek.Select(p => p.Player).ToList();

            var myPlayerInGameToRemove = myPlayersGame
                .Where(p => !playersModelId.Contains(p.PlayerId));
            this.Data.PlayersGames.DeleteRange(p => myPlayerInGameToRemove.Contains(p));

            var gamePlayersInGameweekToRemove = gamePlayersGameweek
                .Where(p => !playersModelId.Contains(p.PlayerId));
            this.Data.PlayersGamesGameweeks.DeleteRange(g => gamePlayersInGameweekToRemove.Contains(g));

            this.Data.SaveChanges();

            foreach (var player in myPlayersToConfirm)
            {
                var playerFromModel = playersModel.FirstOrDefault(p => p.Id == player.Id);

                if (!myPlayersInGame.Contains(player))
                {                    
                    var playerInGame = new PlayersGame()
                    {
                        GameId = id,
                        PlayerId = player.Id,
                        GamePlayer = gamePlayer,
                        IsStarting = playerFromModel.IsStarting
                    };

                    this.Data.PlayersGames.Add(playerInGame);
                }

                if (!gamePlayersInGameweek.Contains(player))
                {
                    string aginstTeam = GetTeamAgainstInGameweek(player);

                    var playerInGameweek = new PlayersGamesGameweek()
                    {
                        GameweekId = currentGameweekId.Value,
                        PlayerId = player.Id,
                        GameId = game.Id,
                        GamePlayer = gamePlayer,
                        TeamAgainst = aginstTeam,
                        IsStarting = playerFromModel.IsStarting
                    };

                    this.Data.PlayersGamesGameweeks.Add(playerInGameweek);
                }
            }

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

            return PartialView("_TransferPitch", playersModel);
        }

        private static int GetMyPlayersCount(PlayerGameViewModel[] playersModel)
        {
            int myPlayersCount = 0;
            foreach (var cell in playersModel)
            {
                if (cell != null)
                {
                    myPlayersCount++;
                }
            }
            return myPlayersCount;
        }

        public ActionResult OpponentTeam(int id)
        {
            var game = this.Data.Games.GetById(id);
            string userId = this.User.Identity.GetUserId();
            GamePlayer gamePlayer;
            decimal opponentBudget = 0;
            string opponentName;
            if (game.First_PlayerId != userId)
            {
                opponentBudget = game.FirstUserBudget;
                opponentName = game.FirstUser.UserName;
                gamePlayer = GamePlayer.FirstPlayer;
            }
            else
            {
                opponentBudget = game.SecondUserBudget;
                opponentName = game.SecondUser.UserName;
                gamePlayer = GamePlayer.SecondPlayer;
            }

            ViewBag.OpponentUserName = opponentName;
            ViewBag.Budget = opponentBudget;
            ViewData["Fixtures"] = PopulateCurrentMatches();
            var opponentPlayers = this.Data.PlayersGames.All().Where(p => p.GameId == id &&
                 p.GamePlayer == gamePlayer).Select(PlayerGameViewModel.FromPlayersGame).ToList();

            Dictionary<int, List<PlayerGameViewModel>> players = PlayersInDictionary(opponentPlayers);

            return View(GetStartingPlayers(players));
        }

        public ActionResult GameweekResult(int id)
        {
            var game = this.Data.Games.GetById(id);
            currentGameweekId = this.Data.Gameweeks.All()
                .FirstOrDefault(g => g.StartDate <= DateTime.Now && 
                    DateTime.Now <= g.Deadline).Id;
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
            var playersInGameweek = this.Data.PlayersGamesGameweeks.All()
                .Where(p => p.GameId == id && p.GameweekId == previousGameweekId);

            if (game.GameState != GameState.InProgress || playersInGameweek.Count() == 0)
            {
                return View(results);
            }

            var playersList = playersInGameweek.Select(g => g.Player).ToList();

            var playersInGameweekToList = playersInGameweek.ToList();
            for (int i = 0; i < playersInGameweekToList.Count; i++)
            {
                var currentPlayer = playersList.FirstOrDefault(p => p.Id ==
                    playersInGameweekToList[i].PlayerId);
                playersInGameweekToList[i].Points = currentPlayer.RoundScore;
            }

            this.Data.SaveChanges();

            var firstUserPlayersList = playersInGameweek.Where(u => u.GamePlayer ==
                GamePlayer.FirstPlayer).Select(PlayerGameViewModel.FromPlayersGameweek).ToList();
            var secondUserPlayersList = playersInGameweek.Where(u => u.GamePlayer ==
                GamePlayer.SecondPlayer).Select(PlayerGameViewModel.FromPlayersGameweek).ToList();

            Dictionary<int, List<PlayerGameViewModel>> firstUserPlayers =
                PlayersInDictionary(firstUserPlayersList);
            var firstUserStartingPlayers = GetStartingPlayers(firstUserPlayers);

            Dictionary<int, List<PlayerGameViewModel>> secondUserPlayers =
                PlayersInDictionary(secondUserPlayersList);
            var secondUserStartingPlayers = GetStartingPlayers(secondUserPlayers);
            game.FirstUserPoints -= game.FirstUserGWPoints;
            game.FirstUserGWPoints = CalculateTotalPoints(firstUserPlayersList);
            game.FirstUserPoints += game.FirstUserGWPoints;
            game.SecondUserPoints -= game.SecondUserGWPoints;
            game.SecondUserGWPoints = CalculateTotalPoints(secondUserPlayersList);
            game.SecondUserPoints += game.SecondUserGWPoints;
            
            this.Data.SaveChanges();
            ViewBag.FirstPlayerPoints = game.FirstUserPoints;
            ViewBag.SecondPlayerPoints = game.SecondUserPoints;

            ViewBag.FirstPlayerGWPoints = game.FirstUserGWPoints;
            ViewBag.SecondPlayerGWPoints = game.SecondUserGWPoints;

            results.FirstUserPlayers = firstUserStartingPlayers;
            results.SecondUserPlayers = secondUserStartingPlayers;
            ViewBag.GameweekMatches = this.Data.Matches.All()
                .Where(m => m.GameweekId == previousGameweekId)
                .Select(MatchViewModel.FromMatches).ToList();

            return View(results);
        }

        private string GetTeamAgainstInGameweek(Player player)
        {
            var playerMatchInGameweek = this.Data.Matches.All()
                .Where(m => m.GameweekId == currentGameweekId)
                    .FirstOrDefault(x => x.Host_TeamId == player.TeamId ||
                        x.Visitor_TeamId == player.TeamId);
            string aginstTeam;
            if (player.TeamId == playerMatchInGameweek.Host_TeamId)
            {
                aginstTeam = "(H)" + playerMatchInGameweek.Visitor.Initials;
            }
            else
            {
                aginstTeam = "(A)" + playerMatchInGameweek.Host.Initials;
            }
            return aginstTeam;
        }

        private bool IsStarting(Player myPlayer)
        {
            switch (myPlayer.Position)
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

            var myPlayersInGame = this.Data.PlayersGames.All().Where(p => p.GameId == id &&
                p.GamePlayer == gamePlayer);
            var myPlayers = myPlayersInGame.Select(PlayerGameViewModel.FromPlayersGame).ToList();

            foreach (var player in myPlayersInGame)
            {
                var againstTeam = GetTeamAgainstInGameweek(player.Player);
                myPlayers.FirstOrDefault(p => p.Id == player.PlayerId)
                    .AgainstTeamInGameweek = againstTeam;
            }

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