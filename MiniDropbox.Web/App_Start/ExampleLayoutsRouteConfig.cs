using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BootstrapMvcSample.Controllers;
using MiniDropbox.Web.Controllers;
using NavigationRoutes;

namespace BootstrapMvcSample
{
    public class ExampleLayoutsRouteConfig
    {
       
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapNavigationRoute<DiskController>("Home", c => c.ListAllContent());

            routes.MapNavigationRoute<ListAccountController>("Opciones de Perfil", c => c.Index())
                  .AddChildRoute<AccountController>("Perfil", c => c.UpdatePerfil())
                  .AddChildRoute<ListAccountController>("Usered register", c => c.ListAccount())
                  .AddChildRoute<AccountController>("Logout", c => c.LogIn())
                ;
        }
     }
}
