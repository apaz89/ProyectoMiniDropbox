using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapMvcSample;
using BootstrapMvcSample.Controllers;
using BootstrapSupport;
using FizzWare.NBuilder;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
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
        TransaccionesUrl urls = new TransaccionesUrl();
        static Account raccAccount = new Account();
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
            if (model.Password != model.ConfirPassword)
            {
                Mjserror = "";
                return RedirectToAction("Register", "Account");
            }

            var sExiste = _readOnlyRepository.GetAccountEmail(model.Email);
            if (sExiste == null)
            {
               raccAccount=new Account()
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
            ViewData["ErrorForgot"] = Mjserror;
            Mjserror = "";
            return View(new AccountForgotPasswordModel());
        }

        [HttpPost]
        public ActionResult ForgotPassword(AccountForgotPasswordModel model)
        {
            var sExiste = _readOnlyRepository.GetAccountEmail(model.Email);
            if (sExiste != null)
            {
                urls.Account_Id = sExiste.Id;
                urls.IsArchived = sExiste.IsArchived;
                urls.token = System.Guid.NewGuid().ToString();
                urls.type = "resetPass";
                _writeOnlyRepository.BeginTransaccion();
                var sTransUrl = _writeOnlyRepository.Create(urls);
                sendmail.Limpiar();
                sendmail.EnviarA(model.Email);
                sendmail.Subject("Reset Password en MiniDropbox.com");
                sendmail.Body("Ingrese a este link para cambiar su contraseña: http://localhost:13913/Account/ResetPassword?Token="+urls.token);
                if (!sendmail.Enviar())
                {
                    Mjserror = "No se pudo enviar email..";
                    _writeOnlyRepository.RollBackTransaccion();
                }
                else
                {
                    Mjserror = "Se envio un correo con el link para cambiar su contraseña..";
                    _writeOnlyRepository.CommitTransaccion();
                }
            }
            else
            {
                Mjserror = "Su email no esta registrado en nuestro sistema";
            }
            return RedirectToAction("ForgotPassword", "Account");
        }

        [HttpGet]
        public ActionResult ResetPassword(string Token)
        {
            ViewData["ErrorReset"] = Mjserror;
            Mjserror = "";
            raccAccount=_readOnlyRepository.GetAccountwithToken(Token);
            return View(new ResetPasswordModel());
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (model.NuevoPassword != model.ConfirmePassword)
            {
                Mjserror = "La confirmacion de la contraseña no es igual";
            }
            else
            {
                raccAccount.Password = decode.encryptMe(model.NuevoPassword);
                raccAccount.Nombre = "Pedro";
                _writeOnlyRepository.Update(raccAccount);
                Mjserror = "Contraseña cambiada correctamente, proceda a ingresar";
            }
            return RedirectToAction("ResetPassword","Account");
        }
    }
}