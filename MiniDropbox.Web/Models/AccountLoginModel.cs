using System.ComponentModel.DataAnnotations;
using Microsoft.Web.Mvc;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace MiniDropbox.Web.Models
{
    public class AccountLoginModel
    {
        [Required(ErrorMessage = "Este campo no puede quedar vacio")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Este campo no puede quedar vacio")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}