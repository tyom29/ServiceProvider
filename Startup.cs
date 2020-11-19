using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ServiceProvider.Startup))]
namespace ServiceProvider
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
