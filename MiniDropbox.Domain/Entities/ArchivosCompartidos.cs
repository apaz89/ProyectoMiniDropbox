using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniDropbox.Domain.Entities
{
    public class ArchivosCompartidos : IEntity
    {
        private readonly IList<File> _files = new List<File>();

        public ArchivosCompartidos()
        {
            _files = new List<File>();
        }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string usercompartio { get; set; }
        public virtual string userrecibe { get; set; }
        public virtual IEnumerable<File> Files
        {
            get { return _files; }
        }
        public virtual void AddFile(File file)
        {
            if (!_files.Contains(file))
            {
                _files.Add(file);
            }
        }
    }

}
