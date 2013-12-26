using FantasyStatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FantasyStatsApp.Models.GameModels;
using FantasyStats.Model;

namespace FantasyStatsApp.Controllers
{
    public class GamesController : BaseController
    {
        //
        // GET: /Games/
        public ActionResult MyGames()
        {
            var userId = this.User.Identity.GetUserId();
            var myGames = this.Data.Games.All().Where(g => g.First_PlayerId == userId || g.Second_PlayerId == userId)
              .Select(x => new GameBasicViewModel()
              {
                  Id = x.Id,
                  Name = x.Name
              }).ToList();

            return View(myGames);
        }

        public ActionResult JoinGames()
        {
            var userId = this.User.Identity.GetUserId();
            var òpenGames = this.Data.Games.All()
                .Where(g => g.First_PlayerId != userId && g.Second_PlayerId != userId &&
                    g.GameState == GameState.Created)
              .Select(x => new GameBasicViewModel()
              {
                  Id = x.Id,
                  Name = x.Name,
                  GameState = x.GameState
              }).ToList();

            return View(òpenGames);
        }

        public ActionResult Create()
        {           
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GameCreateModel game)
        {
            if (ModelState.IsValid)
            {
                var newGame = new Game();
                newGame.Name = game.Name;
                newGame.GameState = GameState.Created;
                newGame.First_PlayerId = this.User.Identity.GetUserId();
                this.Data.Games.Add(newGame);
                this.Data.SaveChanges();
                return RedirectToAction("PickTeam", "GameInfo", new { id = newGame.Id });
            }

            return View(game);
        }

        //public ActionResult Details(int id)
        //{
        //    var players = this.Data.PlayersGames.All().Where(p => p.GameId == id)
        //        .Select(x => x.Player).Select(p => PlayerBasicModel.FromPlayersStats);
        //    return View(players);
        //}
	}
}