using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Web.Mvc;

namespace MiniDropbox.Web.Models
{
    public class FileModel
    {
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Tamaño")]
        public long Tamanio { get; set; }

        [Display(Name = "Tipo")]
        public string Tipo { get; set; }

        [Display(Name = "Fecha Subio")]
        public DateTime FechaSubio { get; set; }

        [Display(Name = "Fecha Modifico")]
        public DateTime FechaModifico { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }

    }
}