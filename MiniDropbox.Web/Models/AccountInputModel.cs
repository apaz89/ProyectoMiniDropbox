using System.ComponentModel.DataAnnotations;
using Microsoft.Web.Mvc;

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
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}