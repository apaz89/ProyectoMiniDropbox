using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniDropbox.Web.Models
{
    public class ListAccountModel
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public long Consumo  { get; set; }
        public long EspacioAsignado { get; set; }
        public bool Estado { get; set; }
    }
}