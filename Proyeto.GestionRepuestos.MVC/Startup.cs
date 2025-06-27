using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Proyeto.GestionRepuestos.MVC.Startup))]
namespace Proyeto.GestionRepuestos.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
