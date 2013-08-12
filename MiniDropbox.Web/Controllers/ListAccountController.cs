using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Clases;
using MiniDropbox.Web.Models;

namespace MiniDropbox.Web.Controllers
{
    public class ListAccountController : Controller
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
        public ActionResult ListAccount()
        {
            var listOfContent = _readOnlyRepository.AllItemsRead<Account>().ToList();
            return View(listOfContent);
        }

        public ActionResult Index()
        {
            return RedirectToAction("ListAllContent", "Disk");
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            ViewData["ErrorEditCuenta"] = Mjserror;
            Session["IdEdit"] = id;
            Mjserror = "";
           
            ListAccountModel model = new ListAccountModel();
            var _temp = _readOnlyRepository.GetById<Account>(id);
            model.Nombre = _temp.Nombre;
            model.Email = _temp.Email;
            model.Consumo = _temp.Consumo;
            model.EspacioAsignado = _temp.EspacioAsignado;
            model.Apellido = _temp.Apellido;
            return View("AccountDetails", model);
        }

        public string Mjserror { get; set; }

        [HttpPost]
        public ActionResult Edit(ListAccountModel model)
        {
            if (model.Consumo < model.EspacioAsignado)
            {
                Mjserror = "El espacio asignado no puede ser menor que el consumo";
            }
            else
            {
                var raccAccount = _readOnlyRepository.GetById<Account>((long)Session["IdEdit"]);
                if (raccAccount != null)
                {
                    raccAccount.EspacioAsignado = model.EspacioAsignado;
                    try
                    {
                        _writeOnlyRepository.BeginTransaccion();
                        _writeOnlyRepository.Update<Account>(raccAccount);
                        Mjserror = "Espacio cambiado correctamente";
                        sendmail.Limpiar();
                        sendmail.EnviarA(raccAccount.Email);
                        sendmail.Subject("Se ha modificado su espacio");
                        sendmail.Body(" Su espacio disponible en nuestro Mini DropBox es de:"+model.EspacioAsignado);
                        sendmail.Enviar();
                        _writeOnlyRepository.CommitTransaccion();
                    }
                    catch (Exception ee)
                    {
                        _writeOnlyRepository.RollBackTransaccion();
                        Mjserror = ee.Message;
                    }
                }
            }
            return View("AccountDetails", model);
        }

        [HttpPost]
        public ActionResult BloquearCuenta()
        {

            var raccAccount = _readOnlyRepository.GetById<Account>((long)Session["IdEdit"]);
                if (raccAccount != null)
                {
                    raccAccount.Estado = false;
                    try
                    {
                        _writeOnlyRepository.BeginTransaccion();
                        _writeOnlyRepository.Update<Account>(raccAccount);
                        Mjserror = "Se Bloqueo Correctamente";
                        sendmail.Limpiar();
                        sendmail.EnviarA(raccAccount.Email);
                        sendmail.Subject("Cuenta Bloqueada en MiniDropbox");
                        sendmail.Body(" Se ha desactivado su cuenta, contactese con nosotros para volver activarla");
                        sendmail.Enviar();
                        _writeOnlyRepository.CommitTransaccion();
                    }
                    catch (Exception ee)
                    {
                        _writeOnlyRepository.RollBackTransaccion();
                        Mjserror = ee.Message;
                    }
                }

            return RedirectToAction("ListAccount");
        }


    }
}
