using System.ComponentModel.DataAnnotations;

namespace MiniDropbox.Web.Models
{
    public class AccountInputModel
    {
        [Display(Name="Nombre")]
        public string Name { get; set; }
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Este campo no debe estar vacio")]
        public string Email { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}