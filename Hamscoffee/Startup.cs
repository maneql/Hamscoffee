using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hamscoffee.Startup))]
namespace Hamscoffee
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
