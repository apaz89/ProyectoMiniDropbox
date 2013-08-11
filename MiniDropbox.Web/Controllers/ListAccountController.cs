using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Models;

namespace MiniDropbox.Web.Controllers
{
    public class ListAccountController : Controller
    {
        //
        // GET: /ListAccount/

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

    }
}
