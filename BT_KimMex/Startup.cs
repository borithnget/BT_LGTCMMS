using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BT_KimMex.Startup))]
namespace BT_KimMex
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseErrorPage();
            CreateRoles();
            /*
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = "/login",
            });
            */
        }
        private void CreateRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            //if (!roleManager.RoleExists("Price Manager"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Price Manager";
            //    roleManager.Create(role);
            //}
            //if (!roleManager.RoleExists("Main Stock Controller"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Main Stock Controller";
            //    roleManager.Create(role);
            //}
            //if (!roleManager.RoleExists("Economic Engineer"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Economic Engineer";
            //    roleManager.Create(role);
            //}
            //if (!roleManager.RoleExists("Purchaser"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Purchaser";
            //    roleManager.Create(role);
            //}
            //if (!roleManager.RoleExists("Chief of Finance Officer"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Chief of Finance Officer";
            //    roleManager.Create(role);
            //}
            //if (!roleManager.RoleExists("Stock Keeper"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Stock Keeper";
            //    roleManager.Create(role);
            //}
        }
    }
}
