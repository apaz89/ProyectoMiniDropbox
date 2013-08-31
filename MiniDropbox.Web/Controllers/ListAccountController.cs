using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BootstrapMvcSample.Controllers;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Clases;
using MiniDropbox.Web.Models;
using Newtonsoft.Json.Serialization;

namespace MiniDropbox.Web.Controllers
{
    public class ListAccountController : BootstrapBaseController
    {
        //
        // GET: /ListAccount/
        private Clases.EnvioEmail sendmail = new EnvioEmail();

        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public ListAccountController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        [HttpGet]
        public ActionResult AllListAccount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var listOfContent = _readOnlyRepository.AllItemsRead<Account>().ToList();
            return View(listOfContent);
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("AllListAccount", "ListAccount");
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (id != 0)
                Session["IdEdit"] = id;

            var temp = _readOnlyRepository.GetById<Account>(id);
            var model = Mapper.Map<Account,ListAccountModel>(temp);
            return View("AccountDetails", model);
        }

        [HttpPost]
        public ActionResult Edit(ListAccountModel model)
        {
            if (model.Consumo > model.EspacioAsignado)
            {
                Error("El espacio asignado no puede ser menor que el consumo");
            }
            else
            {
                var raccAccount = _readOnlyRepository.GetById<Account>((long)Session["IdEdit"]);
                if (raccAccount != null)
                {
                    raccAccount.EspacioAsignado = model.EspacioAsignado;
                    try
                    {
                        _writeOnlyRepository.Update<Account>(raccAccount);
                        Error("Espacio cambiado correctamente");
                        #region EnviarMail
                        sendmail.Limpiar();
                        sendmail.EnviarA(raccAccount.Email);
                        sendmail.Subject("Se ha modificado su espacio");
                        sendmail.Body(" Su espacio disponible en nuestro Mini DropBox es de:"+model.EspacioAsignado +" Gb");
                        sendmail.Enviar();
                        #endregion 
                    }
                    catch (Exception ee)
                    {
                       Error(ee.Message);
                    }
                }
            }
            return RedirectToAction("Edit", "ListAccount");
        }

        [HttpGet]
        public ActionResult BloquearCuenta(long id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            
            if (id != 0)
                Session["IdEdit"] = id;
            
            var temp = _readOnlyRepository.GetById<Account>(id);
            var model = Mapper.Map<Account, ListAccountModel>(temp);
            model.Nombre = temp.Nombre;
            model.Email = temp.Email;
            model.Consumo = temp.Consumo;
            model.EspacioAsignado = temp.EspacioAsignado;
            model.Apellido = temp.Apellido;

            return View("BloquearCuenta", model);
        }
        
        [HttpPost]
        public ActionResult BloquearCuenta(ListAccountModel model)
        {
            var raccAccount = _readOnlyRepository.GetById<Account>((long) Session["IdEdit"]);
            if (raccAccount != null)
            {
                raccAccount.Estado = false;
                try
                {
                    _writeOnlyRepository.Update<Account>(raccAccount);
                    Error("Se Bloqueo Correctamente");

                    #region EnviarEmail
                    sendmail.Limpiar();
                    sendmail.EnviarA(raccAccount.Email);
                    sendmail.Subject("Cuenta Bloqueada en MiniDropbox");
                    sendmail.Body(" Se ha desactivado su cuenta, contactese con nosotros para volver activarla");
                    sendmail.Enviar();
                    #endregion
                }
                catch (Exception ee)
                {
                    Error(ee.Message);
                }
            }

            return RedirectToAction("BloquearCuenta", "ListAccount");
        }
    }
}
