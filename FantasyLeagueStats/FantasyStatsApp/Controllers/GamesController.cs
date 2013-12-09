using FantasyStats.Model;
using FantasyStatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FantasyStatsApp.Controllers
{
    public class GamesController : BaseController
    {
        //
        // GET: /Games/
        public ActionResult Index()
        {
            return View();
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
                return RedirectToAction("/Index");
            }

            return View(game);
        }

        public ActionResult MyGames()
        {
            var myGames = this.Data.Games.All().Where(g => g.FirstUser == this.User || g.SecondUser == this.User)
                .Select(x => new GameBasicViewModel() 
                {
                    Id = x.Id,
                    Name = x.Name
                });

            return PartialView("_MyGames", myGames);
        }
	}
}