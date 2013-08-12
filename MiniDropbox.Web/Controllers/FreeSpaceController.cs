using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using System.Web.UI;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Clases;
using MiniDropbox.Web.Models;

namespace MiniDropbox.Web.Controllers
{
    public class FreeSpaceController : Controller
    {
        //
        // GET: /FreeSpace/

        private Clases.EnvioEmail sendmail = new EnvioEmail();
        private Clases.Utilidades util = new Utilidades();
        private TransaccionesUrl urls = new TransaccionesUrl();
        private static string Mjserror;
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public FreeSpaceController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }   

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InviteFriends()
        {
            ViewData["InviteFriend"] = Mjserror;
            Mjserror = "";
            return View(new InviteFriendsModel());
        }

        [HttpPost]
        public ActionResult InviteFriends(InviteFriendsModel model)
        {
            long _temp_id = util._AccountActual.Id;
            var sExiste = _readOnlyRepository.GetById<Account>(_temp_id);
            if (sExiste != null)
            {
                urls.Account_Id = sExiste.Id;
                urls.IsArchived = sExiste.IsArchived;
                urls.token = System.Guid.NewGuid().ToString();
                urls.type = "invitedFriend";
                _writeOnlyRepository.BeginTransaccion();
                var sTransUrl = _writeOnlyRepository.Create(urls);
                sendmail.Limpiar();
                sendmail.EnviarA(model.Email);
                if (model.Email1 != string.Empty)
                    sendmail.EnviarA(model.Email);
                if (model.Email2 != string.Empty)
                    sendmail.EnviarA(model.Email);
                if (model.Email3 != string.Empty)
                    sendmail.EnviarA(model.Email);
                sendmail.Subject("Invitacion a MiniDrobox por:" +util._AccountActual.Nombre);
                sendmail.Body("Has sido inivatado a usar Mini dropbox por tu amigo:" + util._AccountActual.Nombre + " Ingrese a este link para registrarse: http://minidropbox-2.apphb.com/Account/Register?Token=" + urls.token);
         
                if (!sendmail.Enviar())
                {
                    Mjserror = "No se pudo enviar email..";
                    _writeOnlyRepository.RollBackTransaccion();
                }
                else
                {
                    Mjserror = "Se ha enviado las invitaciones";
                    _writeOnlyRepository.CommitTransaccion();
                }
            }
            return RedirectToAction("InviteFriends","FreeSpace");
        }


        [HttpGet]
        public ActionResult Home()
        {
            return RedirectToAction("ListAllContent", "Disk");
        }
    }

}

