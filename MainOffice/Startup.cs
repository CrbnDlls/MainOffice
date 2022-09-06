using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MainOffice.Startup))]
namespace MainOffice
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
