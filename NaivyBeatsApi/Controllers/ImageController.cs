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
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using NaivyBeatsApi.Models;

namespace NaivyBeatsApi.Controllers
{
    public class ImageController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // GET: api/Image/{path}
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
    }
}