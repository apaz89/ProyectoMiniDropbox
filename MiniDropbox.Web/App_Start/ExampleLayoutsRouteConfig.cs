using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BootstrapMvcSample.Controllers;
using BootstrapSupport;
using MiniDropbox.Web.Controllers;
using NavigationRoutes;

namespace BootstrapMvcSample
{
    public class ExampleLayoutsRouteConfig
    {
       
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapNavigationRoute<DiskController>("Home", c => c.ListAllContent());
            routes.MapNavigationRoute<FreeSpaceController>("Free Space", c => c.Index())
                .AddChildRoute<FreeSpaceController>("Invite Friends", c => c.InviteFriends());
                //.AddChildRoute<FreeSpaceController>("up in class", c => c.UpInClass());
            routes.MapNavigationRoute<ListAccountController>("Opciones de Perfil", c => c.Index())
                .AddChildRoute<AccountController>("Perfil", c => c.UpdatePerfil())
                .AddChildRoute<ListAccountController>("Usuarios Registrados", c => c.AllListAccount())
                .AddChildRoute<PaquetesPremiumController>("Paquetes Premium", c => c.Paquetes())
                .AddChildRoute<AccountController>("Logout", c => c.Logout());
        }

     
    }
}
