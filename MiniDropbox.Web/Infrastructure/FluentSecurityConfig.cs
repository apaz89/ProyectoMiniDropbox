using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using BootstrapMvcSample.Controllers;
using FluentSecurity;
using MiniDropbox.Web.Controllers;
using MiniDropbox.Web.Models;

namespace MiniDropbox.Web.Infrastructure
{
    public static class SecurityHelpers
    {
        public static IEnumerable<object> UserRoles()
        {
            var listRoles = new List<string> { "Admin", "User" };
            return listRoles;
        }
    }

    public static class FluentSecurityConfig
    {
        public static void Configure()
        {
            SecurityConfigurator.Configure(configuration =>
            {
                configuration.GetAuthenticationStatusFrom(() => HttpContext.Current.User.Identity.IsAuthenticated);
                configuration.GetRolesFrom(SecurityHelpers.UserRoles);

                configuration.ForAllControllers().DenyAnonymousAccess();
                configuration.For<AccountController>(x => x.LogIn()).Ignore();
                configuration.For<AccountController>(x => x.ForgotPassword()).Ignore();
                configuration.For<AccountController>(x => x.ResetPassword(new ResetPasswordModel() )).Ignore();
                configuration.For<AccountController>(x => x.Register(new AccountInputModel() )).Ignore();
                configuration.For<AccountController>(x => x.UpdatePerfil()).DenyAnonymousAccess();
                configuration.For<FreeSpaceController>(x => x.Index()).DenyAnonymousAccess();
                configuration.For<FreeSpaceController>(x => x.InviteFriends()).DenyAnonymousAccess();
                configuration.For<ListAccountController>(x => x.AllListAccount()).DenyAnonymousAccess();
                configuration.For<ListAccountController>(x => x.Edit(new long())).DenyAnonymousAccess();
                configuration.For<PaquetesPremiumController>(x => x.Edit(new long())).DenyAnonymousAccess();
                configuration.For<PaquetesPremiumController>(x => x.CreatePaquete()).DenyAnonymousAccess();
                configuration.For<PaquetesPremiumController>(x => x.DesactivarPaquete(new long())).DenyAnonymousAccess();

                //configuration.For<DiskController>(x => x.index()).RequireRole(new object[] { "Admin" });
                configuration.ResolveServicesUsing(type =>
                {
                    if (type == typeof(IPolicyViolationHandler))
                    {
                        var types = Assembly
                            .GetAssembly(typeof(MvcApplication))
                            .GetTypes()
                            .Where(x => typeof(IPolicyViolationHandler).IsAssignableFrom(x)).ToList();

                        var handlers = types.Select(t => Activator.CreateInstance(t) as IPolicyViolationHandler).ToList();

                        return handlers;
                    }
                    return Enumerable.Empty<object>();
                });
            });

        }
    }
}