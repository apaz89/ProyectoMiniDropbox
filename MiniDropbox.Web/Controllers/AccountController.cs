using System.Linq;
using System.Web.Mvc;
using BootstrapMvcSample;
using BootstrapMvcSample.Controllers;
using FizzWare.NBuilder;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Clases;
using MiniDropbox.Web.Models;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MiniDropbox.Web.Controllers
{
    public class AccountController : BootstrapBaseController
    {
        private static string Mjserror;
        private EncryptString.Encrypt decode = new EncryptString.Encrypt();
        private Clases.EnvioEmail sendmail = new EnvioEmail();
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public AccountController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            ViewData["ErrorPass"] = Mjserror;
            Mjserror = "";
            return View(new AccountLoginModel());    
            
        }

        [HttpPost]
        public ActionResult LogIn(AccountLoginModel mode)
        {
            ///validar que el login es valido

            var account = _readOnlyRepository.GetAccountEmail(mode.Username);
            if (account == null)
            {
                Mjserror= "No existe el Usuario, vuelva a intentar de nuevo";
            }
            else
            {
                if (decode.decryptMe(account.Password) == mode.Password)
                {
                    ExampleLayoutsRouteConfig.NameHome = account.Nombre;
                    return RedirectToAction("ListAllContent", "Disk");
                }
                else
                {
                   Mjserror = "Contraseña Invalida";
                }
            }
            return RedirectToAction("LogIn", "Account");
        }

        [HttpPost]
        public ActionResult Register(AccountInputModel model)
        {

            ////////valido si existe el usuario

            var sExiste = _readOnlyRepository.GetAccountEmail(model.Email);
            if (sExiste == null)
            {
                var raccAccount = new Account
                {
                    Nombre = model.Nombre,
                    Email = model.Email,
                    Consumo = 0,
                    Estado = true,
                    EspacioAsignado = 2,
                    Password = decode.encryptMe(model.Password),
                    Tipo = "Estandar",
                    Apellido = model.Apellido
                };
                _writeOnlyRepository.BeginTransaccion();
                var register = _writeOnlyRepository.Create(raccAccount);
                sendmail.Limpiar();
                sendmail.EnviarA(model.Email);
                sendmail.Subject("Register en MiniDropbox.com");
                sendmail.Body("Te as registrado exitosamente en MiniDropbox.com, ahora podras tener tus archivos en la nuve,  empieza ya!");
                if (!sendmail.Enviar())
                {
                    Mjserror = "No se pudo registrar..";
                    _writeOnlyRepository.RollBackTransaccion();
                    return RedirectToAction("Register", "Account");
                }
                else
                {
                    _writeOnlyRepository.CommitTransaccion();
                }
            }
            else
            {
                Mjserror = "La cuenta ya existe..";
                return RedirectToAction("Register", "Account");
            }

            return RedirectToAction("ListAllContent", "Disk");
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewData["ErrorReg"] = Mjserror;
            Mjserror = "";
            return View(new AccountInputModel());
        }

        
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View(new AccountForgotPasswordModel());
        }

    }

    
}