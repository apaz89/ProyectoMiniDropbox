using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Models;

namespace MiniDropbox.Web.Controllers
{
    public class PaquetesPremiumController : Controller
    {
        //
        // GET: /PaquetesPremium/

        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public PaquetesPremiumController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }


        [HttpGet]
        public ActionResult Paquetes()
        {
            var listOfContent = _readOnlyRepository.AllItemsRead<PaquetesPremium>().ToList();
            return View(listOfContent);
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            ViewData["ErrorPaqPremium"] = Mjserror;
            if (id != 0)
            {
                Session["IdEditPremium"] = id;
            }
            Mjserror = "";

            var model = new PaquetesPremiumModel();;
            var _temp = _readOnlyRepository.GetById<PaquetesPremium>(id);
            model.NombrePaquete = _temp.Nombre;
            model.Descripcion = _temp.Descripcion;
            model.Precio = _temp.Precio;
            model.CantidadDias = _temp.CantidadDias;
            model.CantidadEspacio = _temp.CantidadEspacio;
            model.Estado = _temp.Estado;
            return View("PaquetesPremiunEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(PaquetesPremiumModel model)
        {
                var _paquetesPremium = _readOnlyRepository.GetById<PaquetesPremium>((long)Session["IdEditPremium"]);
                if (_paquetesPremium != null)
                {
                    _paquetesPremium.Nombre = model.NombrePaquete;
                    _paquetesPremium.Descripcion = model.Descripcion;
                    _paquetesPremium.Precio = model.Precio;
                    _paquetesPremium.CantidadDias = model.CantidadDias;
                    _paquetesPremium.CantidadEspacio = model.CantidadEspacio;

                    try
                    {
                        _writeOnlyRepository.BeginTransaccion();
                        _writeOnlyRepository.Update<PaquetesPremium>(_paquetesPremium);
                        Mjserror = "Cambios realizados correctamente";
                       _writeOnlyRepository.CommitTransaccion();
                    }
                    catch (Exception ee)
                    {
                        _writeOnlyRepository.RollBackTransaccion();
                        Mjserror = ee.Message;
                    }
                }
            
            return RedirectToAction("Edit", "PaquetesPremium");
        }



        public static string Mjserror { get; set; }

        [HttpGet]
        public ActionResult DesactivarPaquete(long id)
        {
            ViewData["ErrorPaqPremium"] = Mjserror;
            if (id != 0)
            {
                Session["IdEditPremium"] = id;
            }
            Mjserror = "";

            var model = new PaquetesPremiumModel(); ;
            var _temp = _readOnlyRepository.GetById<PaquetesPremium>(id);
            model.NombrePaquete = _temp.Nombre;
            model.Descripcion = _temp.Descripcion;
            model.Precio = _temp.Precio;
            model.CantidadDias = _temp.CantidadDias;
            model.CantidadEspacio = _temp.CantidadEspacio;
            model.Estado = _temp.Estado;
            return View("DesactivarPaquetesPremiunEdit", model);
        }

        [HttpPost]
        public ActionResult DesactivarPaquete(PaquetesPremiumModel model)
        {
            var _paquetesPremium = _readOnlyRepository.GetById<PaquetesPremium>((long)Session["IdEditPremium"]);
            if (_paquetesPremium != null)
            {
                _paquetesPremium.Estado = false;
                try
                {
                    _writeOnlyRepository.BeginTransaccion();
                    _writeOnlyRepository.Update<PaquetesPremium>(_paquetesPremium);
                    Mjserror = "Paquete desactivado correctamente";
                    _writeOnlyRepository.CommitTransaccion();
                }
                catch (Exception ee)
                {
                    _writeOnlyRepository.RollBackTransaccion();
                    Mjserror = ee.Message;
                }
            }

            return RedirectToAction("DesactivarPaquete", "PaquetesPremium");
        }

        [HttpGet]
        public ActionResult CreatePaquete()
        {
            ViewData["ErrorPaqPremium"] = Mjserror;
            Mjserror = "";

            return View(new PaquetesPremiumModel());
        }

        [HttpPost]
        public ActionResult CreatePaquete(PaquetesPremiumModel model)
        {
            var _paquetesPremium =new PaquetesPremium
            {
                Nombre = model.NombrePaquete,
                Descripcion = model.Descripcion,
                Precio = model.Precio,
                CantidadDias = model.CantidadDias,
                CantidadEspacio = model.CantidadEspacio,
                Estado=true
            };
            try
            {
                _writeOnlyRepository.BeginTransaccion();
                _writeOnlyRepository.Create<PaquetesPremium>(_paquetesPremium);
                Mjserror = "Se guardo correctamente";
                _writeOnlyRepository.CommitTransaccion();
            }
            catch (Exception ee)
            {
                _writeOnlyRepository.RollBackTransaccion();
                Mjserror = ee.Message;
            }

            return RedirectToAction("CreatePaquete", "PaquetesPremium");
        }
    }
}
