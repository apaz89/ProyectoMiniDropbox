using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Antlr.Runtime.Tree;

namespace MiniDropbox.Web.Clases
{
    public class DirectoriosArchivos
    {
        private string _path;

        public string PathActual
        {
            get
            {
                return _path;

            }
            set
            {
                _path = value;
            }
        }


        public DirectoriosArchivos(string path)
        {
            _path = path;
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public void AddSubFolder(string name)
        {
            var dir = new DirectoryInfo(_path + @"\" +name);
            if (!dir.Exists)
                dir.Create();
        }

        public bool SubirArchivo(HttpPostedFileBase fichero)
        {
            try
            {
                fichero.SaveAs(Path.Combine(_path, Path.GetFileName(fichero.FileName)));
            }
            catch (Exception ee)
            {
                return false;
            }
            return true;
        }


        public bool EliminarFolder(string url)
        {
            try
            {
                Directory.Delete(_path + url, true);
            }
            catch (Exception ee)
            {
                return false;
            }
            return true;
        }

        public bool EliminarFile(string url)
        {
            try
            {
                if(File.Exists(url))
                {
                    File.Delete(_path + url);
                }
            }
            catch (Exception ee)
            {
                return false;
            }
            return true;
        }

    }
}