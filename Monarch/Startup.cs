using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Monarch.Startup))]
namespace Monarch
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
