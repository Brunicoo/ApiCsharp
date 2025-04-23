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
using NaivyBeatsApi.DTOS;
using NaivyBeatsApi.Models;

namespace NaivyBeatsApi.Controllers
{
    public class PublicationController: ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();


        // GET: api/Publication/5
        [ResponseType(typeof(List<PostLike>))]
        [Route("api/Publication/{user_id}")]
        public List<PostLike> getAllPublications(int user_id)
        {
            List<Publication> posts = db.Publication.ToList();
            List<PostLike> postsLikes = new List<PostLike>();
            foreach(var post in posts)
            {
                PostLike pl = new PostLike();
                pl.description = post.description;
                pl.titulo = post.titulo;
                pl.multimedia_content = post.multimedia_content;
                pl.publication_date = post.publication_date;
                pl.publication_id = post.publication_id;
                pl.user_id = post.user_id;

                var like = db.Like_Restaurant_Publication.FirstOrDefault(lr => lr.restaurant_id == user_id);

                if (like != null)
                {
                    pl.like = 1;
                } else
                {
                    pl.like = 0;
                }

                var chat = db.Chat.FirstOrDefault(ch => ch.musician_id == pl.user_id && ch.restaurant_id == user_id);

                if (chat != null)
                {
                    pl.chat = 1;
                } else
                {
                    pl.chat = 0;
                }

                var follow = db.Follow.FirstOrDefault(f => f.musician_id == pl.user_id && f.restaurant_id == user_id);

                if (follow != null)
                {
                    pl.follow = 1;
                } else
                {
                    pl.follow = 0;
                }
                
                postsLikes.Add(pl);
            }

            return postsLikes;
        }

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
                string relativePath = Path.Combine("Data", "Publications");
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

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