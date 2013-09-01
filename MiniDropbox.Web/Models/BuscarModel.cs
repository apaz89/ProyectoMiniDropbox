using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Web.Mvc;

namespace MiniDropbox.Web.Models
{
    public class BuscarModel
    {
        [Display(Name = "Nombre: ")]
        public string Nombre { get; set; }
    }
}