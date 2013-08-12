using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniDropbox.Domain.Entities
{
    public class ReferredUsersList:IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual long Account_id { get; set; }
        public virtual long Account_Id_refered { get; set; }
    }
}
