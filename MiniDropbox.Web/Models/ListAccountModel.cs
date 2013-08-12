using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NHibernate.Type;

namespace MiniDropbox.Web.Models
{
    public class ListAccountModel
    {
        [Editable(false)]
        [Display(Name="Nombre")]
        public string Nombre { get; set; }
        [Editable(false)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }
        [Editable(false)]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Editable(false)]
        [Range(0, 1000)]
        [Display(Name = "Consumo Gb")]
        public long Consumo  { get; set; }
        [Range(0,1000)]
        [Display(Name = "Espacio Gb")]
        public long EspacioAsignado { get; set; } 
    }
}