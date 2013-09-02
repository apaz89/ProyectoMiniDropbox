using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniDropbox.Domain.Entities
{
    public class FolderPublico : IEntity
    {
        private readonly IList<File> _files = new List<File>();
        public FolderPublico()
        {
            _files = new List<File>();
        }
        public virtual IEnumerable<File> Files
        {
            get { return _files; }
        }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string Token { get; set; }
        public virtual long Folder_Id { get; set; }
        public virtual long Account_Id { get; set; }
        public virtual long Drive_Id { get; set; }
    }
}
