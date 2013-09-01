using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniDropbox.Web.Models
{
    public class SubirArchivoModel
    {
        [Display(Name = "Subir Archivo")]
        public string Nombre { get; set; }
    }
}