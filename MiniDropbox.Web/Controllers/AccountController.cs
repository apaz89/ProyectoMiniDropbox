using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BootstrapMvcSample;
using BootstrapMvcSample.Controllers;
using AutoMapper;
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

        #region variables

        private EncryptString.Encrypt decode = new EncryptString.Encrypt();
        private Clases.EnvioEmail sendmail = new EnvioEmail();
        private TransaccionesUrl urls = new TransaccionesUrl();
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        #endregion

        public AccountController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn");
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View(new AccountLoginModel());
        }

        [HttpPost]
        public ActionResult LogIn(AccountLoginModel mode)
        {
            var account = _readOnlyRepository.GetAccountEmail(mode.Username);
            if (account == null)
            {
                Error("No existe el Usuario, vuelva a intentar de nuevo");
            }
            else
            {
                if (account.Estado)
                {
                    if (decode.decryptMe(account.Password) == mode.Password)
                    {
                        List<string> roles = account.Roles.Select(x => x.Name).ToList();
                        FormsAuthentication.SetAuthCookie(mode.Username, mode.RememberMe);
                        SetAuthenticationCookie(mode.Username,roles);
                        return RedirectToAction("ListAllContent", "Disk");
                    }
                    else
                    {
                        Error("Contraseña Invalida");
                    }
                }
                else
                {
                    Error("Cuenta esta bloqueada");
                }
            }
            return View(mode);
        }

        [HttpGet]
        public ActionResult Register(string Token)
        {
            if (Token != null)
            {
                Session["TokenInvite"] = Token;
            }
            return View(new AccountInputModel());
        }

        [HttpPost]
        public ActionResult Register(AccountInputModel model)
        {
            ////////valido si existe el usuario
            try
            {
                if (model.Password != model.ConfirPassword)
                {
                    return RedirectToAction("Register", "Account");
                }

                var sExiste = _readOnlyRepository.GetAccountEmail(model.Email);
                if (sExiste == null)
                {
                    var account = Mapper.Map<AccountInputModel, Account>(model);
                    account.Password = decode.encryptMe(account.Password);
                    account.Estado = true;
                    var register = _writeOnlyRepository.Create(account);

                    //verico si es una invitacion

                    #region InvitacionFriend

                    var tokentmp = (string) Session["TokenInvite"];

                    if (tokentmp != null)
                    {
                        account = _readOnlyRepository.GetAccountwithToken(tokentmp);
                        if (account != null)
                        {
                            account.EspacioAsignado = account.EspacioAsignado + 1;
                            _writeOnlyRepository.Update<Account>(account);

                            ////insertar en lista de usuarios referidos
                            var refUsers = new ReferredUsersList
                            {
                                Account_Id_refered = register.Id,
                                Account_id = account.Id
                            };
                            _writeOnlyRepository.Create(refUsers);
                        }
                    }

                    #endregion

                    //Envio un email

                    #region sendemail

                    sendmail.Limpiar();
                    sendmail.EnviarA(model.Email);
                    sendmail.Subject("Register en MiniDropbox.com");
                    sendmail.Body(
                        "Te as registrado exitosamente en MiniDropbox.com, ahora podras tener tus archivos en la nuve,  empieza ya!");
                    if (!sendmail.Enviar())
                    {
                        Error("No se pudo registrar..");
                        _writeOnlyRepository.RollBackTransaccion();
                        return RedirectToAction("Register", "Account");
                    }

                    #endregion
                }
                else
                {
                    Error("La cuenta ya existe..");
                    return RedirectToAction("Register", "Account");
                }
            }
            catch (Exception ee)
            {
                Error(ee.Message);
            }

            return RedirectToAction("ListAllContent", "Disk");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
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

                sendmail.Body(
                    "Ingrese a este link para cambiar su contraseña: http://localhost:13913/Account/ResetPassword?Token=" +
                    urls.token);

                sendmail.Body("Ingrese a este link para cambiar su contraseña: http://minidropbox-2.apphb.com/Account/ResetPassword?Token=" + urls.token);

                if (!sendmail.Enviar())
                {
                    Error("No se pudo enviar email..");
                    _writeOnlyRepository.RollBackTransaccion();
                }
                else
                {
                    Information("Se envio un correo con el link para cambiar su contraseña..");
                    _writeOnlyRepository.CommitTransaccion();
                }
            }
            else
            {
                Error("Su email no esta registrado en nuestro sistema");
            }
            return RedirectToAction("ForgotPassword", "Account");
        }

        [HttpGet]
        public ActionResult ResetPassword(string Token)
        {
            if (Token != null)
            {
                Session["Token"] = Token;
            }
            else
            {
                return RedirectToAction("LogIn");
            }
            return View(new ResetPasswordModel());
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (model.NuevoPassword != model.ConfirmePassword)
            {
                Error("La confirmacion de la contraseña no es igual");
            }
            else
            {
                var raccAccount = _readOnlyRepository.GetAccountwithToken(Session["Token"].ToString());
                if (raccAccount != null)
                {
                    raccAccount.Password = decode.encryptMe(model.NuevoPassword);
                    try
                    {
                        _writeOnlyRepository.Update<Account>(raccAccount);
                        Success("Contraseña cambiada correctamente, proceda a ingresar");
                    }
                    catch (Exception ee)
                    {
                        Error(ee.Message);
                    }
                }
                else
                {
                    Error("No exite enlace, para cambiar su contraseña");
                }
            }
            return RedirectToAction("ResetPassword", "Account");
        }

        [HttpGet]
        public ActionResult UpdatePerfil()
        {
            if (User.Identity.IsAuthenticated)
            {

                var model = new UpdatePerfilModel();
                var temp = _readOnlyRepository.First<Account>(x => x.Email == User.Identity.Name);
                Mapper.Map<Account, UpdatePerfilModel>(temp, model);
                model.Password = decode.decryptMe(temp.Password);
                model.ConfirPassword = model.Password;
                return View("UpdatePerfil", model);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        [HttpPost]
        public ActionResult UpdatePerfil(UpdatePerfilModel model)
        {
            try
            {
                if (model.ConfirPassword != model.Password)
                {
                    Error("La confirmacion de la contraseña no es igual");
                }
                else
                {
                    var temp = _readOnlyRepository.First<Account>(x => x.Email == User.Identity.Name);
                    Mapper.Map<UpdatePerfilModel, Account>(model, temp);
                    temp.Password = decode.encryptMe(model.Password);
                    _writeOnlyRepository.Update<Account>(temp);
                    Success("Cambios salvados correctamente");
                }
            }
            catch (Exception ee)
            {
                Error(ee.Message);
            }

            return RedirectToAction("UpdatePerfil", "Account");
        }

        [HttpGet]
        public ActionResult Home()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login");
            }
            else
            {
                return RedirectToAction("ListAllContent", "Disk");
            }
        }

    }
}

                                                                               
