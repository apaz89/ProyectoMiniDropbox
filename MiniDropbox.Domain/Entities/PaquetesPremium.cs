using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniDropbox.Domain
{
    public class PaquetesPremium : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string  Nombre { get; set; }
        public virtual string  Descripcion { get; set; }
        public virtual double Precio { get; set; }
        public virtual long CantidadEspacio { get; set; }
        public virtual long CantidadDias { get; set; }
        public virtual bool Estado { get; set; }
    }   
}
