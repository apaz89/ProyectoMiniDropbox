using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapMvcSample.Controllers;
using BootstrapSupport;
using FizzWare.NBuilder;
using Microsoft.Ajax.Utilities;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Clases;
using MiniDropbox.Web.Models;
using File = MiniDropbox.Domain.File;

namespace MiniDropbox.Web.Controllers
{
    public class DiskController : BootstrapBaseController
    {
        private Clases.DirectoriosArchivos DirArch;
        private string urlActual;
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public DiskController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        public DiskController()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult ListAllContent()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var account = _readOnlyRepository.First<Account>(x => x.Email == User.Identity.Name);

            Session["PATH"] = Server.MapPath("~/Directorios") + @"\" + account.Id.ToString(CultureInfo.InvariantCulture);
            Session["FOLDER"] = account.Id.ToString(CultureInfo.InvariantCulture);

            DirArch =
                new DirectoriosArchivos(Server.MapPath("~/Directorios") + @"\" +
                                        account.Id.ToString(CultureInfo.InvariantCulture));

            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
            var folder =
                _readOnlyRepository.First<Folder>(x => x.FolderName == account.Id.ToString(CultureInfo.InvariantCulture) && x.IsArchived!=true);

            List<DiskContentModel> list = new List<DiskContentModel>();
            if (drive != null)
            {
// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var folSu in folder.Subfolders)
                {
                    if (folSu.FolderName != (string) Session["FOLDER"])
                    {
                        var dcont = new DiskContentModel {Name = folSu.FolderName, Id = folSu.Id, IsFolder = true};
                        list.Add(dcont);
                    }
                }

// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var files in folder.Files)
                {
                    if (files.IsArchived != true)
                    {
                        var dcont = new DiskContentModel
                        {
                            Name = files.Nombre,
                            Id = files.Id,
                            Type = files.Tipo,
                            ModifiedDate = files.FechaModifico
                        };
                        list.Add(dcont);
                    }
                }
            }
            Session["Ver"] = null;
            var listOfContent = list;
            return View(listOfContent);
        }

        [HttpGet]
        public ActionResult SubirArchivo()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View("SubirArchivo");
        }

        [HttpPost]
        public ActionResult SubirArchivo(string descripcion, HttpPostedFileBase fichero)
        {
            if (fichero != null)
            {
                try
                {
                    if (fichero.FileName != null)
                    {
                        if (fichero.ContentLength > 10485760)
                        {
                            Error("Fichero no puede ser mayor de 10 MB");
                        }
                        else
                        {
                            DirArch = new DirectoriosArchivos((string) Session["PATH"]);
                            fichero.SaveAs(Path.Combine(DirArch.PathActual, Path.GetFileName(fichero.FileName)));
                            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
                            var fin = new FileInfo(Path.GetFileName(DirArch.PathActual + @"\" + fichero.FileName));
                            var account = _readOnlyRepository.First<Account>(x => x.Email == User.Identity.Name);
                            var archivo = _readOnlyRepository.First<File>(x => x.Nombre == fichero.FileName && x.IsArchived != true);

                            if (archivo == null)
                            {
                                var folder =
                                    _readOnlyRepository.First<Folder>(x => x.FolderName == (string)Session["FOLDER"] && x.IsArchived != true);

                                var file = new File
                                {
                                    Nombre = fin.Name,
                                    Tamanio = fichero.ContentLength,
                                    Url = DirArch.PathActual + @"\" + fichero.FileName,
                                    Tipo = fin.Extension,
                                    IsArchived = false,
                                    FechaModifico = DateTime.Now,
                                    FechaSubio = DateTime.Now,
                                    Folder_id = folder.Id,
                                    Drive_id = drive.Id,
                                    Account_id = account.Id
                                };
                                _writeOnlyRepository.Create(file);
                            }
                            else
                            {
                                archivo.Tamanio = fichero.ContentLength;
                                archivo.FechaModifico = DateTime.Now;
                                _writeOnlyRepository.Update<File>(archivo);
                            }
                            Success("Archivo subido correctamente");
                        }
                    }
                    else
                        Error("No ha seleccionado archivos");
                }
                catch (Exception ee)
                {
                    Error(ee.Message);
                }
            }
            else
            {
                Information("Archivo no seleccionado");
            }
            return RedirectToAction("SubirArchivo");
        }

        [HttpGet]
        public ActionResult NuevoDirectorio(string s)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(new FolderModel());
        }

        [HttpPost]
        public ActionResult NuevoDirectorio(FolderModel model)
        {
            var folder = _readOnlyRepository.First<Folder>(x => x.FolderName == model.Nombre && x.IsArchived != true);
            if (folder == null)
            {
                DirArch = new DirectoriosArchivos((string) Session["PATH"] + @"\" + model.Nombre);
                var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
                var existefolder = _readOnlyRepository.First<Folder>(x => x.FolderName == (string)Session["FOLDER"] && x.IsArchived != true);

                var fold = new Folder(model.Nombre)
                {
                    IsArchived = false,
                    Drive_Id = drive.Id,
                    Folder_Id = existefolder.Id,
                    Url = (string)Session["PATH"] + @"\" + model.Nombre
                };
                _writeOnlyRepository.Create(fold);
                Success("Folder Creado Correctamente");
            }
            else
            {
                Error("Carpeta ya existe");
                return RedirectToAction("NuevoDirectorio");
            }

            return RedirectToAction("ListAllContent");
        }
  
        [HttpGet]
        public ActionResult Ver(long id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var account = _readOnlyRepository.First<Account>(x => x.Email == User.Identity.Name);
            var folderActuaL = _readOnlyRepository.First<Folder>(x => x.Id == id && x.IsArchived != true);
            Session["PATH"] = (string) Session["PATH"] + @"\" + folderActuaL.FolderName;
            Session["FOLDER"] = folderActuaL.FolderName;
            DirArch = new DirectoriosArchivos((string) Session["PATH"]);
            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
            ViewData["Nombre"] = folderActuaL.FolderName;
            Session["Ver"] = id;
            List<DiskContentModel> list = new List<DiskContentModel>();
            if (drive != null)
            {
// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var folder in folderActuaL.Subfolders)
                {
                    if (folder.IsArchived != true)
                    {
                        var dcont = new DiskContentModel {Name = folder.FolderName, Id = folder.Id, IsFolder = true};
                        list.Add(dcont);
                    }
                }

// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var arch in folderActuaL.Files)
                {
                    if (arch.IsArchived != true)
                    {
                        var dfil = new DiskContentModel
                        {
                            Name = arch.Nombre,
                            Id = arch.Id,
                            Type = arch.Tipo,
                            ModifiedDate = arch.FechaModifico
                        };
                        list.Add(dfil);
                    }
                }
            }
            var listOfContent = list;
            return View(listOfContent);
        }

        [HttpGet]
        public ActionResult Delete(long id)
        {
            Attention("Si ELimina un archivo no podra recuperarlo");
            var folder = _readOnlyRepository.First<Folder>(x => x.Id == id && x.IsArchived != true);
            if (folder != null)
            {
                folder.IsArchived = false;
                foreach (var file in folder.Files)
                {
                    file.IsArchived = true;
                    _writeOnlyRepository.Update<File>(file);
                }
                folder.IsArchived = true;
                _writeOnlyRepository.Update<Folder>(folder);
                Directory.Delete(folder.Url, true);
                Success("Folder Eliminado correctamente");
            }
            else
            {
                var file = _readOnlyRepository.First<File>(x => x.Id == id && x.IsArchived != true);
                if (file != null)
                {
                    file.IsArchived = true;
                    _writeOnlyRepository.Update<File>(file);
                    System.IO.File.Delete(file.Url);
                    Success("Archivo Eliminado correctamente");
                }
                else
                {
                    Error("No existe archivo a elmiminar");
                }
            }
            return RedirectToAction("ListAllContent");
        }

        [HttpGet]
        public ActionResult Buscar()
        {
            var listofconten = _readOnlyRepository.Query<File>(x => x.Nombre == "").ToList();
            if (Session["Buscar"] != null)
            {
                listofconten = _readOnlyRepository.Query<File>(x => x.Nombre.Contains(Session["Buscar"].ToString())).ToList();
            }
            return View(listofconten);
        }

        [HttpPost]
        public ActionResult Buscar(string descripcion)
        {
            Session["Buscar"] = descripcion;
            return RedirectToAction("Buscar");
        }
    }
}