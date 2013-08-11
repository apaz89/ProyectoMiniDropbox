using System.ComponentModel.DataAnnotations;
using Microsoft.Web.Mvc;
using NHibernate.Bytecode;

namespace MiniDropbox.Web.Models
{
    public class AccountInputModel
    {

        [Display(Name="Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Este campo no debe estar vacio")]
        [EmailAddress(ErrorMessage = "Debe tener Formato de Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Este campo no debe estar vacio")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long and maximum {1} characters long", MinimumLength = 6)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Este campo no debe estar vacio")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long and maximum {1} characters long", MinimumLength = 6)]
        [Display(Name = "Confirmed Password")]
        [DataType(DataType.Password)]
        public string ConfirPassword { get; set; }

    }
}