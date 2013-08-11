using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniDropbox.Web.Models
{
    public class ResetPasswordModel
    {

        [Display(Name = "Nueva Contraseña: ")]
        [Required(ErrorMessage = "Este campo no puede quedar vacio")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long and maximum {1} characters long", MinimumLength = 6)]
        public string NuevoPassword { get; set; }
        [Display(Name = "Confirme Contraseña: ")]
        [Required(ErrorMessage = "Este campo no puede quedar vacio")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long and maximum {1} characters long", MinimumLength = 6)]
        public string ConfirmePassword { get; set; }
    }
}