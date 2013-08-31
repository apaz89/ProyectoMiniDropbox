using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniDropbox.Web.Models
{
    public class PaquetesPremiumModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long and maximum {1} characters long",MinimumLength = 6)]
        [Display(Name = "Nombre del Paquete")]
        public string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long and maximum {1} characters long",MinimumLength = 25)]
        [Display(Name = "Descripcion")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        public double Precio { get; set; }

        [Display(Name = "Cantidad Espacio")]
        public long CantidadEspacio { get; set; }

        [Display(Name = "Cantidad Espacio")]
        public long CantidadDias { get; set; }

        [Display(Name = "Estado de Paquete")]
        public bool Estado { get; set; }
    }
}