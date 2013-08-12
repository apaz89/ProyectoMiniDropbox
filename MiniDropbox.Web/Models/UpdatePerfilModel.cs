using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Web.Mvc;

namespace MiniDropbox.Web.Models
{
    public class UpdatePerfilModel
    {
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Este campo no debe estar vacio")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Este campo no debe estar vacio")]
        [Display(Name = "Confirmed Password")]
        [DataType(DataType.Password)]
        public string ConfirPassword { get; set; }
        [Display(Name = "Consumo")]
        [ReadOnly(true)]
        public long Consumo { get; set; }
        [Display(Name = "Espacio Asignado")]
        [ReadOnly(true)]
        public long EspacioAsignado { get; set; }
    }
}