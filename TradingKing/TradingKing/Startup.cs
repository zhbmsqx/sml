using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TradingKing.Startup))]
namespace TradingKing
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
