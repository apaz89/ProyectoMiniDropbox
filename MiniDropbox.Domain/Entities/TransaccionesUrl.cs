using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace MiniDropbox.Domain.Entities
{
    public class TransaccionesUrl:IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual long Account_Id { get; set; }
        public virtual string token { get; set; }
        public virtual string type { get; set; }//para cambio contraseña y invitaciones
    }
}
