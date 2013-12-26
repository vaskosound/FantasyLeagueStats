using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace FantasyStatsApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

          routes.MapRoute(
                name: "GameInfo",
                url: "GameInfo/{id}/{action}",
                defaults: new { controller = "GameInfo" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
