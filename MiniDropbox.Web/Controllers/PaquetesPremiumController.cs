using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using BootstrapMvcSample.Controllers;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Models;

namespace MiniDropbox.Web.Controllers
{
    public class PaquetesPremiumController : BootstrapBaseController
    {
        //
        // GET: /PaquetesPremium/

        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public PaquetesPremiumController(IReadOnlyRepository readOnlyRepository,
            IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        public ActionResult NoExiste()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public ActionResult Paquetes()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!User.IsInRole("Admin"))
            {
                return View("NoExiste");
            }

            var listOfContent = _readOnlyRepository.AllItemsRead<PaquetesPremium>().ToList();
            return View(listOfContent);
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (id != 0)
                Session["IdEditPremium"] = id;

            var temp = _readOnlyRepository.GetById<PaquetesPremium>(id);
            var model = Mapper.Map<PaquetesPremium, PaquetesPremiumModel>(temp);

            return View("PaquetesPremiunEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(PaquetesPremiumModel model)
        {
            var paquetesPremium = _readOnlyRepository.GetById<PaquetesPremium>((long) Session["IdEditPremium"]);
            if (paquetesPremium != null)
            {
                Mapper.Map<PaquetesPremiumModel, PaquetesPremium>(model, paquetesPremium);
                try
                {
                    _writeOnlyRepository.Update<PaquetesPremium>(paquetesPremium);
                    Error("Cambios realizados correctamente");
                }
                catch (Exception ee)
                {
                    Error(ee.Message);
                }
            }

            return RedirectToAction("Edit", "PaquetesPremium");
        }

        [HttpGet]
        public ActionResult DesactivarPaquete(long id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (id != 0)
                Session["IdEditPremium"] = id;

            var temp = _readOnlyRepository.GetById<PaquetesPremium>(id);
            var model = Mapper.Map<PaquetesPremium, PaquetesPremiumModel>(temp);
            return View("DesactivarPaquetesPremiunEdit", model);
        }

        [HttpPost]
        public ActionResult DesactivarPaquete(PaquetesPremiumModel model)
        {
            var paquetesPremium = _readOnlyRepository.GetById<PaquetesPremium>((long) Session["IdEditPremium"]);

            if (paquetesPremium != null)
            {
                paquetesPremium.Estado = false;
                try
                {
                    _writeOnlyRepository.Update<PaquetesPremium>(paquetesPremium);
                    Error("Paquete desactivado correctamente");
                }
                catch (Exception ee)
                {
                    Error(ee.Message);
                }
            }
            return RedirectToAction("DesactivarPaquete", "PaquetesPremium");
        }

        [HttpGet]
        public ActionResult CreatePaquete()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            return View(new PaquetesPremiumModel());
        }

        [HttpPost]
        public ActionResult CreatePaquete(PaquetesPremiumModel model)
        {
            var paquetesPremium = Mapper.Map<PaquetesPremiumModel, PaquetesPremium>(model);
            paquetesPremium.Estado = true;
            try
            {
                _writeOnlyRepository.Create<PaquetesPremium>(paquetesPremium);
                Error("Se guardo correctamente");
            }
            catch (Exception ee)
            {
                Error(ee.Message);
            }

            return RedirectToAction("CreatePaquete", "PaquetesPremium");
        }
    }
}

