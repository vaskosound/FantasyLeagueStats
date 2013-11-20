using System;
using System.Data.Entity.Migrations;
using System.Linq;
using FantasyStats.Model;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FantasyStats.Data.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<FantasyStats.Data.FantasyStatsDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(FantasyStats.Data.FantasyStatsDbContext context)
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
            if (context.Teams.Count() > 0)
            {
                return;
            }

            Team arsenal = new Team()
            {
                Name = "Arsenal",
                Initials = "ARS"
            };
            context.Teams.Add(arsenal);

            Team astonVilla = new Team()
            {
                Name = "Aston Villa",
                Initials = "AVL"
            };
            context.Teams.Add(astonVilla);

            Team chelsea = new Team()
            {
                Name = "Chelsea",
                Initials = "CHE"
            };
            context.Teams.Add(chelsea);

            Team cardiff = new Team()
            {
                Name = "Cardiff City",
                Initials = "CAR"
            };
            context.Teams.Add(cardiff);

            Team crystalPalace = new Team()
            {
                Name = "Crystal Palace",
                Initials = "CRY"
            };
            context.Teams.Add(crystalPalace);

            Team everton = new Team()
            {
                Name = "Everton",
                Initials = "EVE"
            };
            context.Teams.Add(everton);

            Team fulham = new Team()
            {
                Name = "Fulham",
                Initials = "FUL"
            };
            context.Teams.Add(fulham);

            Team hull = new Team()
            {
                Name = "Hull City",
                Initials = "HUL"
            };
            context.Teams.Add(hull);

            Team liverpool = new Team()
            {
                Name = "Liverpool",
                Initials = "LIV"
            };
            context.Teams.Add(liverpool);

            Team manCity = new Team()
            {
                Name = "Manchester City",
                Initials = "MCI"
            };
            context.Teams.Add(manCity);

            Team manUnited = new Team()
            {
                Name = "Manchester United",
                Initials = "MUN"
            };
            context.Teams.Add(manUnited);

            Team newcastle = new Team()
            {
                Name = "Newcastle United",
                Initials = "NEW"
            };
            context.Teams.Add(newcastle);

            Team norwich = new Team()
            {
                Name = "Norwich City",
                Initials = "NOR"
            };
            context.Teams.Add(norwich);

            Team southampton = new Team()
            {
                Name = "Southampton",
                Initials = "SOU"
            };
            context.Teams.Add(southampton);

            Team stoke = new Team()
            {
                Name = "Stoke City",
                Initials = "STK"
            };
            context.Teams.Add(stoke);

            Team sundelrland = new Team()
            {
                Name = "Sunderland",
                Initials = "SUN"
            };
            context.Teams.Add(sundelrland);

            Team swansea = new Team()
            {
                Name = "Swansea City",
                Initials = "SWA"
            };
            context.Teams.Add(swansea);

            Team tottenham = new Team()
            {
                Name = "Tottenham Hotspur",
                Initials = "TOT"
            };
            context.Teams.Add(tottenham);

            Team westBrom = new Team()
            {
                Name = "West Bromwich Albion",
                Initials = "WBA"
            };
            context.Teams.Add(westBrom);

            Team westHam = new Team()
            {
                Name = "West Ham United",
                Initials = "WHU"
            };
            context.Teams.Add(westHam);
        }
    }
}
