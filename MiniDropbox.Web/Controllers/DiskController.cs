﻿using System;
using System.Linq;
using System.Web.Mvc;
using BootstrapMvcSample.Controllers;
using FizzWare.NBuilder;
using MiniDropbox.Web.Models;

namespace MiniDropbox.Web.Controllers
{
    public class DiskController : BootstrapBaseController
    {

        [HttpGet]
        public ActionResult ListAllContent()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var listOfContent = Builder<DiskContentModel>.CreateListOfSize(15).Build().ToList(); 
            return View(listOfContent);
        }
    }   
}