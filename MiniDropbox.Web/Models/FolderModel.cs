using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace MiniDropbox.Web.Models
{
    public class FolderModel
    {
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} characters long and maximum {1} characters long", MinimumLength = 1)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

    }
}