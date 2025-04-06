using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using NaivyBeatsApi.Models;

namespace NaivyBeatsApi.Controllers
{
    public class PublicationController: ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();


       
        //[ResponseType(typeof(bool))]
        //[HttpPost]
        //public IHttpActionResult postPublication(PublicationDto publicationDto)
        //{
        //    if (publicationDto == null)
        //    {
        //        return BadRequest("Los datos del formulario son incorrectos o están incompletos.");
        //    }

        //    if (string.IsNullOrEmpty(publicationDto.titulo))
        //    {
        //        return BadRequest("El campo 'titulo' es obligatorio.");
        //    }

        //    if (string.IsNullOrEmpty(publicationDto.description))
        //    {
        //        return BadRequest("El campo 'description' es obligatorio.");
        //    }

        //    if (publicationDto.user_id <= 0)
        //    {
        //        return BadRequest("El campo 'user_id' es obligatorio y debe ser mayor que 0.");
        //    }

        //    if (publicationDto.multimedia_content == null || publicationDto.multimedia_content.ContentLength == 0)
        //    {
        //        return BadRequest("No se ha proporcionado ningún archivo.");
        //    }

        //    try
        //    {
        //        // Crear una nueva publicación
        //        Publication p = new Publication
        //        {
        //            user_id = publicationDto.user_id,
        //            titulo = publicationDto.titulo,
        //            description = publicationDto.description,
        //            publication_date = DateTime.Now.Date.ToString("yyyy-MM-dd")
        //        };

        //        string savedFilePath = SaveFile(publicationDto.multimedia_content, publicationDto.user_id);
        //        if (string.IsNullOrEmpty(savedFilePath))
        //        {
        //            return BadRequest("Error al guardar el archivo.");
        //        }

        //        p.multimedia_content = savedFilePath;

        //        // Guardar la publicación en la base de datos
        //        db.Publication.Add(p);
        //        db.SaveChanges();

        //        return Ok(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Capturar cualquier error inesperado
        //        return InternalServerError(ex);
        //    }
        //}


        // POST: api/Publication
        [ResponseType(typeof(bool))]
        public IHttpActionResult postPublication()
        {
            var file = HttpContext.Current.Request.Files["multimedia_content"];
            if (file == null || file.ContentLength == 0)
            {
                return BadRequest("No se ha proporcionado ningún archivo.");
            }
            int userId = Convert.ToInt32(HttpContext.Current.Request.Form["user_id"]);
            string title = HttpContext.Current.Request.Form["title"];
            string description = HttpContext.Current.Request.Form["description"];

            string savedFilePath = SaveFile(file, userId);

            Publication p = new Publication
            {
                user_id = userId,
                titulo = title,
                description = description,
                publication_date = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                multimedia_content = savedFilePath
            };

            db.Publication.Add(p);
            db.SaveChanges();

            return Ok(true);
        }

        private string SaveFile(HttpPostedFile file, int userId)
        {
            try
            {
                string fullPath = @"C:\Users\bruno\Desktop\NaivyBeatsApi\NaivyBeatsApi\NaivyBeatsApi\Data\publications\";

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new Exception($"Tipo de archivo no permitido. Extensiones válidas: {string.Join(", ", allowedExtensions)}");
                }


                string fileName = $"publication.{userId}{fileExtension}";

 
                string filePath = Path.Combine(fullPath, fileName);
                file.SaveAs(filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar el archivo: {ex.Message}", ex);
            }
        }
    }
}