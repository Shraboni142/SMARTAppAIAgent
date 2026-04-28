using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SMART.Web.Startup))]
namespace SMART.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
