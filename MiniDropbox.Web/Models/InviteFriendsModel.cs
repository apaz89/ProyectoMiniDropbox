using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Web.Mvc;

namespace MiniDropbox.Web.Models
{
    public class InviteFriendsModel
    {
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Este campo no debe estar vacio")]
        [EmailAddress(ErrorMessage = "Debe tener Formato de Email")]
        public string Email { get; set; }
        [Display(Name = "Other E-mail")]
        [EmailAddress(ErrorMessage = "Debe tener Formato de Email")]
        public string Email1 { get; set; }
        [Display(Name = "Other E-mail")]
        [EmailAddress(ErrorMessage = "Debe tener Formato de Email")]
        public string Email2 { get; set; }
        [Display(Name = "Other E-mail")]
        [EmailAddress(ErrorMessage = "Debe tener Formato de Email")]
        public string Email3 { get; set; }
          
    }
}