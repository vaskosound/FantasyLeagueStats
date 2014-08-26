namespace FantasyStats.Data.Migrations
{
    using FantasyStats.Model;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<FantasyStatsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(FantasyStatsDbContext context)
        {
            if (context.Roles.FirstOrDefault() == null)
            {
                var userAdmin = new ApplicationUser()
                {
                    UserName = "admin",
                    PasswordHash = "ACQbq83L/rsvlWq11Zor2jVtz2KAMcHNd6x1SN2EXHs7VuZPGaE8DhhnvtyO10Nf5Q==",

                };
                userAdmin.Roles.Add(new IdentityUserRole()
                {
                    Role = new IdentityRole("Admin")
                }
                 );
                userAdmin.Roles.Add(new IdentityUserRole()
                {
                    Role = new IdentityRole("User")
                }
                );
                userAdmin.Logins.Add(new IdentityUserLogin
                {
                    LoginProvider = "Local",
                    ProviderKey = "admin",
                });
                context.Users.Add(userAdmin);
                context.SaveChanges();
            }
            //if (teams.Count() > 0)
            //{
            //    return;
            //}

            List<Team> teams = new List<Team>();

            Team arsenal = new Team()
            {
                Name = "Arsenal",
                Initials = "ARS",
                Season = 2014
            };
            teams.Add(arsenal);

            Team astonVilla = new Team()
            {
                Name = "Aston Villa",
                Initials = "AVL",
                Season = 2014
            };
            teams.Add(astonVilla);

            Team chelsea = new Team()
            {
                Name = "Chelsea",
                Initials = "CHE",
                Season = 2014
            };
            teams.Add(chelsea);

            Team cardiff = new Team()
            {
                Name = "Cardiff City",
                Initials = "CAR"
            };
            //teams.Add(cardiff);

            Team crystalPalace = new Team()
            {
                Name = "Crystal Palace",
                Initials = "CRY",
                Season = 2014
            };
            teams.Add(crystalPalace);

            Team everton = new Team()
            {
                Name = "Everton",
                Initials = "EVE",
                Season = 2014
            };
            teams.Add(everton);

            Team fulham = new Team()
            {
                Name = "Fulham",
                Initials = "FUL"
            };
            //teams.Add(fulham);

            Team hull = new Team()
            {
                Name = "Hull City",
                Initials = "HUL",
                Season = 2014
            };
            teams.Add(hull);

            Team liverpool = new Team()
            {
                Name = "Liverpool",
                Initials = "LIV",
                Season = 2014
            };
            teams.Add(liverpool);

            Team manCity = new Team()
            {
                Name = "Manchester City",
                Initials = "MCI",
                Season = 2014
            };
            teams.Add(manCity);

            Team manUnited = new Team()
            {
                Name = "Manchester United",
                Initials = "MUN",
                Season = 2014
            };
            teams.Add(manUnited);

            Team newcastle = new Team()
            {
                Name = "Newcastle United",
                Initials = "NEW",
                Season = 2014
            };
            teams.Add(newcastle);

            Team norwich = new Team()
            {
                Name = "Norwich City",
                Initials = "NOR"
            };
            //teams.Add(norwich);

            Team southampton = new Team()
            {
                Name = "Southampton",
                Initials = "SOU",
                Season = 2014
            };
            teams.Add(southampton);

            Team stoke = new Team()
            {
                Name = "Stoke City",
                Initials = "STK",
                Season = 2014
            };
            teams.Add(stoke);

            Team sundelrland = new Team()
            {
                Name = "Sunderland",
                Initials = "SUN",
                Season = 2014
            };
            teams.Add(sundelrland);

            Team swansea = new Team()
            {
                Name = "Swansea City",
                Initials = "SWA",
                Season = 2014
            };
            teams.Add(swansea);

            Team tottenham = new Team()
            {
                Name = "Tottenham Hotspur",
                Initials = "TOT",
                Season = 2014
            };
            teams.Add(tottenham);

            Team westBrom = new Team()
            {
                Name = "West Bromwich Albion",
                Initials = "WBA",
                Season = 2014
            };
            teams.Add(westBrom);

            Team westHam = new Team()
            {
                Name = "West Ham United",
                Initials = "WHU",
                Season = 2014
            };
            teams.Add(westHam);

            Team bunley = new Team()
            {
                Name = "Burnley",
                Initials = "BUR",
                Season = 2014
            };
            teams.Add(bunley);

            Team leicester = new Team()
            {
                Name = "Leicester City",
                Initials = "LEI",
                Season = 2014
            };
            teams.Add(leicester);

            Team qpr = new Team()
            {
                Name = "Queens Park Rangers",
                Initials = "QPR",
                Season = 2014
            };
            teams.Add(qpr);

            context.Teams.AddOrUpdate(t => t.Initials, teams.ToArray());
        }
    }
}
