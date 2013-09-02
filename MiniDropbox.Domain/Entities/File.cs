using System;

namespace MiniDropbox.Domain
{
    public class File : IEntity
    {
        public virtual long? ArchivosCompartidos_id { get; set; }
        public virtual long Id { get; set; }
        public virtual string Nombre { get; set; }
        public virtual long Tamanio { get; set; }
        public virtual string Tipo { get; set; }
        public virtual DateTime FechaSubio { get; set; }
        public virtual DateTime FechaModifico { get; set; }
        public virtual string Url { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual long Account_id { get; set; }
        public virtual long Drive_id { get; set; }
        public virtual long Folder_id { get; set; }
        public virtual bool IsCompartido { get; set; }
    }
}