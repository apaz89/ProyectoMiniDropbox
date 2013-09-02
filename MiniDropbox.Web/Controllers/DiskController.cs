using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using BootstrapMvcSample.Controllers;
using BootstrapSupport;
using FizzWare.NBuilder;
using Microsoft.Ajax.Utilities;
using MiniDropbox.Domain;
using MiniDropbox.Domain.Entities;
using MiniDropbox.Domain.Services;
using MiniDropbox.Web.Clases;
using MiniDropbox.Web.Models;
using NHibernate;
using File = MiniDropbox.Domain.File;

namespace MiniDropbox.Web.Controllers
{
    public class DiskController : BootstrapBaseController
    {
        private Clases.DirectoriosArchivos DirArch;
        private string urlActual;
        private Clases.EnvioEmail sendmail = new EnvioEmail();
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public DiskController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
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
                _readOnlyRepository.First<Folder>(
                    x =>
                        x.FolderName == account.Id.ToString(CultureInfo.InvariantCulture) && x.IsArchived != true &&
                        x.Drive_Id == drive.Id);

            List<DiskContentModel> list = new List<DiskContentModel>();
            if (folder != null)
            {
// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var folSu in folder.Subfolders)
                {
                    if (folSu.FolderName != (string) Session["FOLDER"])
                    {
                        var dcont = new DiskContentModel
                        {
                            Name = folSu.FolderName,
                            Id = folSu.Id,
                            IsFolder = true,
                            IsCompartido = folSu.IsCompartido
                        };
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
                            ModifiedDate = files.FechaModifico,
                            IsCompartido = files.IsCompartido
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
        public ActionResult SubirArchivo(HttpPostedFileBase fichero)
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
                            var archivo =
                                _readOnlyRepository.First<File>(
                                    x => x.Nombre == fichero.FileName && x.IsArchived != true && x.Drive_id == drive.Id);

                            if (archivo == null)
                            {
                                var folder =
                                    _readOnlyRepository.First<Folder>(
                                        x =>
                                            x.FolderName == (string) Session["FOLDER"] && x.IsArchived != true &&
                                            x.Drive_Id == drive.Id);

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
        public ActionResult NuevoDirectorio()
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
            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
            var folder =
                _readOnlyRepository.First<Folder>(
                    x => x.FolderName == model.Nombre && x.IsArchived != true && x.Drive_Id == drive.Id);
            if (folder == null)
            {
                DirArch = new DirectoriosArchivos((string) Session["PATH"] + @"\" + model.Nombre);
                var existefolder =
                    _readOnlyRepository.First<Folder>(
                        x =>
                            x.FolderName == (string) Session["FOLDER"] && x.IsArchived != true && x.Drive_Id == drive.Id);

                var fold = new Folder(model.Nombre);
                fold.IsArchived = false;
                fold.Drive_Id = drive.Id;
                fold.Folder_Id = existefolder.Id;
                fold.Url = (string) Session["PATH"] + @"\" + model.Nombre;
                fold.IsCompartido = false;
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
            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
            var folderActuaL =
                _readOnlyRepository.First<Folder>(x => x.Id == id && x.IsArchived != true && x.Drive_Id == drive.Id);

   
            Session["PATH"] = (string) Session["PATH"] + @"\" + folderActuaL.FolderName;
            Session["FOLDER"] = folderActuaL.FolderName;
            DirArch = new DirectoriosArchivos((string) Session["PATH"]);
            ViewData["Nombre"] = folderActuaL.FolderName;
            Session["Ver"] = id;
            
            List<DiskContentModel> list = new List<DiskContentModel>();

            //ArchivosCompartidos

            if (folderActuaL.FolderName == "Shared")
            {
                ViewData["Nombre"] = folderActuaL.FolderName;
                var archshared = _readOnlyRepository.First<ArchivosCompartidos>(x => x.userrecibe == User.Identity.Name);

                if (archshared != null)
                {
                    foreach (var arch in archshared.Files)
                    {
                        if (arch.IsArchived != true  && arch.IsCompartido==true)
                        {
                            var dfil = new DiskContentModel
                            {
                                Name = arch.Nombre,
                                Id = arch.Id,
                                Type = arch.Tipo,
                                ModifiedDate = arch.FechaModifico,
                                IsCompartido = true,
                                Compartido_por=archshared.usercompartio
                            };
                            list.Add(dfil);
                        }
                    }
                }
            }
            else
            {


                if (drive != null)
                {
// ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var folder in folderActuaL.Subfolders)
                    {
                        if (folder.IsArchived != true)
                        {
                            var dcont = new DiskContentModel
                            {
                                Name = folder.FolderName,
                                Id = folder.Id,
                                IsFolder = true,
                                IsCompartido = folder.IsCompartido
                            };
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
                                ModifiedDate = arch.FechaModifico,
                                IsCompartido = arch.IsCompartido
                            };
                            list.Add(dfil);
                        }
                    }
                }
            }
            var listOfContent = list;
            return View(listOfContent);
        }

        [HttpGet]
        public ActionResult Delete(long id)
        {
            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
            var folder =
                _readOnlyRepository.First<Folder>(x => x.Id == id && x.IsArchived != true && x.Drive_Id == drive.Id);
            
        
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
                var file =
                    _readOnlyRepository.First<File>(x => x.Id == id && x.IsArchived != true && x.Drive_id == drive.Id);
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
            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
            if (Session["Buscar"] != null)
            {
                listofconten =
                    _readOnlyRepository.Query<File>(
                        x => x.Nombre.Contains(Session["Buscar"].ToString()) && x.Drive_id == drive.Id).ToList();
            }
            return View(listofconten);
        }

        [HttpPost]
        public ActionResult Buscar(string descripcion)
        {
            Session["Buscar"] = descripcion;
            return RedirectToAction("Buscar");
        }

        [HttpGet]
        public ActionResult Compartir(long id)
        {
            Session["IDFOLDERCOMPARTIR"] = id;
            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);
            var fold = _readOnlyRepository.First<Folder>(x => x.IsCompartido == true && x.Drive_Id == drive.Id);
            if (fold != null)
            {
                Error("Usted ya compartio la carpeta: " + fold.FolderName + ", no puede volver a compartir");
                return RedirectToAction("ListAllContent");
            }

            return View(new FolderPublicoModel());
        }

        [HttpPost]
        public ActionResult Compartir(FolderPublicoModel model)
        {
            var fold = _readOnlyRepository.First<Folder>(x => x.Id == Convert.ToInt64(Session["IDFOLDERCOMPARTIR"]));
            var account = _readOnlyRepository.First<Account>(x => x.Email == User.Identity.Name);
            var drive = _readOnlyRepository.First<Drive>(x => x.BucketName == User.Identity.Name);

            fold.IsCompartido = true;
            _writeOnlyRepository.Update<Folder>(fold);

            var fpublico = new FolderPublico();
            fpublico.Account_Id = account.Id;
            fpublico.Drive_Id = drive.Id;
            fpublico.Folder_Id = fold.Id;
            fpublico.IsArchived = false;
            fpublico.Token = System.Guid.NewGuid().ToString();
            _writeOnlyRepository.Create<FolderPublico>(fpublico);

            sendmail.Limpiar();
            sendmail.EnviarA(model.Email);
            sendmail.Subject("Invitacion a carpeta publica de Mini Dropbox");

            sendmail.Body(
                "Ingrese a este link para ver los archivos de la carpeta: http://localhost:13913/Disk/RecursosCompartidosPublico?Token=" +
                fpublico.Token);


            if (!sendmail.Enviar())
            {
                Error("No se pudo enviar email..");
            }
            else
            {
                Information("Se envio un correo con la invitacion..");
            }


            Success("Folder Compartido Correctamente");
            return View(new FolderPublicoModel());
        }

        [HttpGet]
        public ActionResult DejarCompartir(long id)
        {
            var folder = _readOnlyRepository.First<Folder>(x => x.Id == id);
            if (folder != null)
            {
                folder.IsCompartido = false;
                _writeOnlyRepository.Update<Folder>(folder);
                Success("Recurso se dejo de compartir correctamente");
            }
            return RedirectToAction("ListAllContent");
        }

        [HttpGet]
        public ActionResult VerPublico(long id)
        {
            var folderActuaL =
                _readOnlyRepository.First<Folder>(x => x.Id == id && x.IsArchived != true && x.IsCompartido == true);

            List<DiskContentModel> list = new List<DiskContentModel>();
            if (folderActuaL != null)
            {
                ViewData["NombrePublic"] = folderActuaL.FolderName;

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var folder in folderActuaL.Subfolders)
                {
                    if (folder.IsArchived != true)
                    {
                        var dcont = new DiskContentModel
                        {
                            Name = folder.FolderName,
                            Id = folder.Id,
                            IsFolder = true,
                            IsCompartido = folder.IsCompartido
                        };
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
                            ModifiedDate = arch.FechaModifico,
                            IsCompartido = arch.IsCompartido
                        };
                        list.Add(dfil);
                    }
                }
            }
            else
            {
                ViewData["NombrePublic"] = "No existe";
                Error("El recurso ya no existe");
            }

            var listOfContent = list;
            return View(listOfContent);
        }

        [HttpGet]
        public ActionResult RecursosCompartidosPublico(string Token)
        {
            Int64 idfol = 0;
            if (Token != null)
            {
                var foldpublico = _readOnlyRepository.First<FolderPublico>(x => x.Token == Token);
                if (foldpublico != null)
                {
                    idfol = foldpublico.Folder_Id;
                }
                else
                {
                    Error("Recurso No existe");
                    return RedirectToAction("Logout", "Account");
                }
            }
            else
            {
                Error("Recurso No existe");
                return RedirectToAction("Logout", "Account");
            }

            return RedirectToAction("VerPublico", new {id = idfol});
        }
        
        [HttpGet]
        public ActionResult CompartirArchivo(long id)
        {
            Session["IDARCHIVOCOMPARTIR"] = id;
            return View(new FolderPublicoModel());
        }

        [HttpPost]
        public ActionResult CompartirArchivo(FolderPublicoModel model)
        {
            var file = _readOnlyRepository.First<File>(x => x.Id == Convert.ToInt64(Session["IDARCHIVOCOMPARTIR"]));

            var exisfshared = _readOnlyRepository.First<ArchivosCompartidos>(x => x.userrecibe == model.Email);
            if (exisfshared != null)
            {
                file.ArchivosCompartidos_id = exisfshared.Id;
                file.IsCompartido = true;
                _writeOnlyRepository.Update<File>(file);
            }
            else
            {
                var fshared = new ArchivosCompartidos();
                fshared.AddFile(file);
                fshared.IsArchived = false;
                fshared.usercompartio = User.Identity.Name;
                fshared.userrecibe = model.Email;
                var cfshared = _writeOnlyRepository.Create<ArchivosCompartidos>(fshared);

                file.ArchivosCompartidos_id = cfshared.Id;
                file.IsCompartido = true;
                _writeOnlyRepository.Update<File>(file);
            }



            #region Envio de Mail o Invitacion

            var accoun = _readOnlyRepository.First<Account>(x => x.Email == model.Email);
            var accounActual = _readOnlyRepository.First<Account>(x => x.Email == User.Identity.Name);
            if (accoun != null)
            {
                sendmail.Limpiar();
                sendmail.EnviarA(model.Email);
                sendmail.Subject("Invitacion a carpeta publica de Mini Dropbox");

                sendmail.Body(
                    "Se ha compartido el archivo: " + file.Nombre +
                    " Ingrese a MiniDropbox, ingrese en su carpeta 'Shared' para descargarlo.");

                if (!sendmail.Enviar())
                {
                    Error("No se pudo enviar email..");
                }
                else
                {
                    Information("Se envio un correo con la invitacion..");
                }
            }
            else
            {
                var urls = new TransaccionesUrl();
                urls.Account_Id = accounActual.Id;
                urls.IsArchived = accounActual.IsArchived;
                urls.token = System.Guid.NewGuid().ToString();
                urls.type = "invitedFriend";

                #region EnviarInvitacion

                var sTransUrl = _writeOnlyRepository.Create(urls);

                sendmail.Limpiar();
                sendmail.EnviarA(model.Email);

                if (model.Email != "")
                    sendmail.EnviarA(model.Email);

                sendmail.Subject("Invitacion a MiniDrobox por:" + User.Identity.Name);
                sendmail.Body("Has sido inivatado a usar Mini dropbox por tu amigo:" + User.Identity.Name + " Ingrese a este link para registrarse: http://localhost:13913/Account/Register?Token=" + urls.token +
                    "  Ya qye se ha compartido el archivo: " + file.Nombre +
                    " Registrese y ingrese a su carpeta 'Shared' para descargarlo.");

                if (!sendmail.Enviar())
                {
                    Error("No se pudo enviar email..");
                }
                else
                {
                    Error("Se ha enviado las invitaciones");
                }
                #endregion
            }

            #endregion

            Success("Archivo Compartido Correctamente");
            return View(new FolderPublicoModel());
        }
        
        [HttpGet]
        public ActionResult DejarCompartirArchivo(long id)
        {
            var file = _readOnlyRepository.First<File>(x => x.Id == id);
            if (file != null)
            {
                file.IsCompartido = false;
                _writeOnlyRepository.Update<File>(file);
                Success("Recurso se dejo de compartir correctamente");
            }
            return RedirectToAction("ListAllContent");
        }
    }
}