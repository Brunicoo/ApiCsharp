using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using NaivyBeatsApi.Models;

namespace NaivyBeatsApi.Controllers
{
    public class ImageController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();
         // GET: api/Image/lolo
         [Route("api/Image/{path}")]
             [HttpGet]
             [ResponseType(typeof(HttpResponseMessage))]
             public HttpResponseMessage DownloadImage(string path)
             {
                 path = HttpUtility.UrlDecode(path);

                 path = path.Replace("_", "/")
                .Replace(",", ".") 
                .Replace("-", ":");

                 if (!File.Exists(path))
                 {
                     return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Archivo no encontrado.");
                 }

                 string extension = Path.GetExtension(path).ToLower();

                 var allowedExtensions = new[]
                 {
                 new { Extension = ".jpg", MimeType = "image/jpeg" },
                 new { Extension = ".jpeg", MimeType = "image/jpeg" },
                 new { Extension = ".png", MimeType = "image/png" },
                 new { Extension = ".gif", MimeType = "image/gif" }
             };

                 var mimeType = allowedExtensions.FirstOrDefault(e => e.Extension == extension)?.MimeType;

                 if (string.IsNullOrEmpty(mimeType))
                 {
                     return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Formato de archivo no soportado.");
                 }

                 byte[] fileBytes = File.ReadAllBytes(path);
                 string fileName = Path.GetFileName(path);

                 var response = new HttpResponseMessage(HttpStatusCode.OK)
                 {
                     Content = new ByteArrayContent(fileBytes)
                 };

                 response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

                 response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                 {
                     FileName = fileName
                 };

                 return response;
             }

        /*
        [Route("api/Image/{path}")]
          [HttpGet]
          [ResponseType(typeof(HttpResponseMessage))]
          public HttpResponseMessage DownloadImage(string path)
          {
              try
              {
                  // Decodificar la ruta de la URL
                  path = HttpUtility.UrlDecode(path);
                path = path.Replace("=", "/").Replace("-", ":").Replace(",", ".");

                  // Definir la ruta relativa donde están las imágenes
                  string relativePath = Path.Combine("Data", "avatar");

          
                  if (string.IsNullOrEmpty(relativePath))
                  {
                      return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No se pudo resolver la ruta del servidor.");
                  }

         
              

                  // Verificar si el archivo existe
                  if (!File.Exists(path))
                  {
                      return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Archivo no encontrado en: {path}");
                  }

                  // Obtener la extensión del archivo
                  string extension = Path.GetExtension(path).ToLower();

                  // Lista de extensiones permitidas con sus tipos MIME
                  var allowedExtensions = new[]
                  {
              new { Extension = ".jpg", MimeType = "image/jpeg" },
              new { Extension = ".jpeg", MimeType = "image/jpeg" },
              new { Extension = ".png", MimeType = "image/png" },
              new { Extension = ".gif", MimeType = "image/gif" }
          };

                  // Obtener el tipo MIME correspondiente
                  var mimeType = allowedExtensions.FirstOrDefault(e => e.Extension == extension)?.MimeType;

                  if (string.IsNullOrEmpty(mimeType))
                  {
                      return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Formato de archivo no soportado.");
                  }

                  // Leer el contenido del archivo
                  byte[] fileBytes = File.ReadAllBytes(path);
                  string fileName = Path.GetFileName(path);

                  // Crear la respuesta HTTP
                  var response = new HttpResponseMessage(HttpStatusCode.OK)
                  {
                      Content = new ByteArrayContent(fileBytes)
                  };

                  // Establecer el tipo de contenido
                  response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

                  // Establecer el encabezado de descarga
                  response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                  {
                      FileName = fileName
                  };

                  return response;
              }
              catch (Exception ex)
              {
                  // Manejar errores y devolver una respuesta de error
                  return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error al procesar la solicitud: {ex.Message}");
              }
          }*/

        // PUT: api/Image
        [HttpPut]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage UpdateImage()
        {
            try
            {
                var file = HttpContext.Current.Request.Files["photo"];
                if (file == null || file.ContentLength == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No se ha proporcionado un archivo válido.");
                }

                var path = HttpContext.Current.Request.Form["path"];
                if (string.IsNullOrWhiteSpace(path))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "La ruta de destino no puede estar vacía.");
                }

                SaveFile(file, path);

                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StreamContent(fileStream);

                string contentType = GetContentType(path);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error al procesar la solicitud: {ex.Message}");
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                default:
                    return "application/octet-stream"; 
            }
        }
        private void SaveFile(HttpPostedFile file, string path)
        {
            try
            {
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file), "El archivo no puede ser nulo.");
                }

                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgumentException("La ruta no puede ser nula o vacía.", nameof(path));
                }

                string fileExtension = Path.GetExtension(file.FileName)?.ToLower();

   
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    throw new Exception($"Tipo de archivo no permitido. Extensiones válidas: {string.Join(", ", allowedExtensions)}");
                }

                string directoryPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                file.SaveAs(path);

            }
            catch (Exception ex)
            {

                throw new Exception($"Error al guardar el archivo: {ex.Message}", ex);
            }
        }

    }
}