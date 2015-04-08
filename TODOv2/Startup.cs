using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TODOv2.Startup))]
namespace TODOv2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
