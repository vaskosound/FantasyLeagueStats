using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FantasyStatsApp.Startup))]
namespace FantasyStatsApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
