using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using System.Web.UI;
using BootstrapMvcSample.Controllers;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Clases;
using MiniDropbox.Web.Models;
using Newtonsoft.Json.Serialization;

namespace MiniDropbox.Web.Controllers
{
    public class FreeSpaceController : BootstrapBaseController
    {
        //
        // GET: /FreeSpace/

        private Clases.EnvioEmail sendmail = new EnvioEmail();
        private TransaccionesUrl urls = new TransaccionesUrl();
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public FreeSpaceController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }   

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpGet]
        public ActionResult InviteFriends()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(new InviteFriendsModel());
        }

        [HttpPost]
        public ActionResult InviteFriends(InviteFriendsModel model)
        {
            var sExiste = _readOnlyRepository.First<Account>(x => x.Nombre == User.Identity.Name);
            if (sExiste != null)
            {
                urls.Account_Id = sExiste.Id;
                urls.IsArchived = sExiste.IsArchived;
                urls.token = System.Guid.NewGuid().ToString();
                urls.type = "invitedFriend";

                #region EnviarMail

                var sTransUrl = _writeOnlyRepository.Create(urls);
                
                sendmail.Limpiar();
                sendmail.EnviarA(model.Email);

                if (model.Email1 != "")
                    sendmail.EnviarA(model.Email1);
                if (model.Email2 != "")
                    sendmail.EnviarA(model.Email2);
                if (model.Email3 != "")
                    sendmail.EnviarA(model.Email3);
                sendmail.Subject("Invitacion a MiniDrobox por:" +User.Identity.Name);
                sendmail.Body("Has sido inivatado a usar Mini dropbox por tu amigo:" + User.Identity.Name + " Ingrese a este link para registrarse: http://localhost:13913/Account/Register?Token=" + urls.token);

                 if (!sendmail.Enviar())
                {
                    Error( "No se pudo enviar email..");
                }
                else
                {
                   Error( "Se ha enviado las invitaciones");
                }
                #endregion
            }
            return RedirectToAction("InviteFriends","FreeSpace");
        }

        [HttpGet]
        public ActionResult Home()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("ListAllContent", "Disk");
        }
    }

}

