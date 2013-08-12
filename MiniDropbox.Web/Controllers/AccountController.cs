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
        private Clases.Utilidades util = new Utilidades();
        private static string Mjserror;
        private EncryptString.Encrypt decode = new EncryptString.Encrypt();
        private Clases.EnvioEmail sendmail = new EnvioEmail();
        TransaccionesUrl urls = new TransaccionesUrl();
        private Account raccAccount = new Account();
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
                if (account.Estado)
                {
                    if (decode.decryptMe(account.Password) == mode.Password)
                    {
                        //ExampleLayoutsRouteConfig.NameHome = account.Nombre;
                        Session["Iduser"] = account.Id;
                        Session["Nombre"] = account.Nombre;
                        util._AccountActual= account;
                        return RedirectToAction("ListAllContent", "Disk");
                    }
                    else
                    {
                        Mjserror = "Contraseña Invalida";
                    }
                }
                else
                {
                    Mjserror = "Cuenta esta bloqueada";
                }
            }
            return RedirectToAction("LogIn", "Account");
        }

        [HttpPost]
        public ActionResult Register(AccountInputModel model)
        {
            ////////valido si existe el usuario
            try
            {
                _writeOnlyRepository.BeginTransaccion();
                if (model.Password != model.ConfirPassword)
                {
                    Mjserror = "";
                    return RedirectToAction("Register", "Account");
                }

                var sExiste = _readOnlyRepository.GetAccountEmail(model.Email);
                if (sExiste == null)
                {
                    raccAccount = new Account()
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
                    var register = _writeOnlyRepository.Create(raccAccount);
                    util._AccountActual = register;

                    //verico si es una invitacion

                    #region InvitacionFriend

                    var tokentmp = (string) Session["TokenInvite"];

                    if (tokentmp != null)
                    {
                        raccAccount = _readOnlyRepository.GetAccountwithToken(tokentmp);
                        if (raccAccount != null)
                        {
                            raccAccount.EspacioAsignado = raccAccount.EspacioAsignado + 1;
                            _writeOnlyRepository.Update<Account>(raccAccount);

                            ////insertar en lista de usuarios referidos
                            var refUsers = new ReferredUsersList
                            {
                                Account_Id_refered = register.Id,
                                Account_id = raccAccount.Id
                            };
                            _writeOnlyRepository.Create(refUsers);
                        }
                    }

                    #endregion

                    sendmail.Limpiar();
                    sendmail.EnviarA(model.Email);
                    sendmail.Subject("Register en MiniDropbox.com");
                    sendmail.Body(
                        "Te as registrado exitosamente en MiniDropbox.com, ahora podras tener tus archivos en la nuve,  empieza ya!");
                    if (!sendmail.Enviar())
                    {
                        Mjserror = "No se pudo registrar..";
                        _writeOnlyRepository.RollBackTransaccion();
                        return RedirectToAction("Register", "Account");
                    }
                    else
                    {
                        Session["Iduser"] = register.Id;
                        _writeOnlyRepository.CommitTransaccion();
                    }
                }
                else
                {
                    Mjserror = "La cuenta ya existe..";
                    _writeOnlyRepository.RollBackTransaccion();
                    return RedirectToAction("Register", "Account");
                }
            }
            catch (Exception ee)
            {
                _writeOnlyRepository.RollBackTransaccion();
                Mjserror = ee.Message;
            }

            return RedirectToAction("ListAllContent", "Disk");
        }

        [HttpGet]
        public ActionResult Register(string Token)
        {
            if (Token != null)
            {
                Session["TokenInvite"] = Token;
            }
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
            if (Token != null)
            {
                Session["Token"] = Token;
            }
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
                raccAccount = _readOnlyRepository.GetAccountwithToken(Session["Token"].ToString());
                if (raccAccount != null)
                {
                    raccAccount.Password = decode.encryptMe(model.NuevoPassword);
                    try
                    {
                        _writeOnlyRepository.BeginTransaccion();
                        _writeOnlyRepository.Update<Account>(raccAccount);
                        _writeOnlyRepository.CommitTransaccion();
                        Mjserror = "Contraseña cambiada correctamente, proceda a ingresar";
                    }
                    catch (Exception ee)
                    {
                        _writeOnlyRepository.RollBackTransaccion();
                        Mjserror = ee.Message;
                    }
                }
            }
            return RedirectToAction("ResetPassword","Account");
        }
        
        [HttpGet]
        public ActionResult UpdatePerfil()
        {
            ViewData["ErrorUpPerfil"] = Mjserror;
            Mjserror = "";
            UpdatePerfilModel model = new UpdatePerfilModel();
            var _temp = _readOnlyRepository.GetById<Account>(util._AccountActual.Id);
            model.Nombre = _temp.Nombre;
            model.Email = _temp.Email;
            model.Consumo = _temp.Consumo;
            model.EspacioAsignado = _temp.EspacioAsignado;
            model.Password = decode.decryptMe(_temp.Password);
            model.ConfirPassword = decode.decryptMe(_temp.Password);
            model.Apellido = _temp.Apellido;
            return View("UpdatePerfil", model);
        }

        [HttpPost]
        public ActionResult UpdatePerfil(UpdatePerfilModel model)
        {
            if (model.ConfirPassword != model.Password)
            {
                Mjserror = "La confirmacion de la contraseña no es igual";
            }
            else
            {
                var _temp = _readOnlyRepository.GetById<Account>(util._AccountActual.Id);
                _temp.Nombre = model.Nombre;
                _temp.Apellido = model.Apellido;
                _temp.Password = decode.encryptMe(model.Password);
                try
                {
                    _writeOnlyRepository.BeginTransaccion();
                    _writeOnlyRepository.Update<Account>(_temp);
                    _writeOnlyRepository.CommitTransaccion();
                    Mjserror = "Cambios salvados correctamente";
                }
                catch (Exception ee)
                {
                    _writeOnlyRepository.RollBackTransaccion();
                    Mjserror = ee.Message;
                }
            }
            return RedirectToAction("UpdatePerfil", "Account");
        }


        [HttpGet]
        public ActionResult Home()
        {
            return RedirectToAction("ListAllContent", "Disk");
        }
    }
}