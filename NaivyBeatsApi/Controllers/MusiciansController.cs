using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Linq;
using NaivyBeatsApi.Models;
using Newtonsoft.Json;

namespace NaivyBeatsApi.Controllers
{
    
    public class MusiciansController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // GET: api/Musicians
        public IEnumerable<Musician> GetMusicians()
        {
            return db.Musician.ToList();
        }

        // GET: api/Musicians/5
        [ResponseType(typeof(Musician))]
        public IHttpActionResult GetMusician(int id)
        {
            Users user = db.Users.Find(id);
           

           Musician m = db.Musician.Find(user.user_id);
            m.name = user.name;
            m.photo = user.photo;
            m.email = user.email;
            m.password = user.password;
            m.phone_number = user.phone_number;
            m.creation_date = user.creation_date;
            m.edition_date = user.edition_date;
            m.deleted_at = user.deleted_at;
            m.municipality_id = user.municipality_id;
            m.latitud = user.latitud;
            m.longitud = user.longitud;

            int[] style_ids = db.User_Style
                    .Where(us => us.user_id == id)
                    .Select(us => us.style_id)
                    .ToArray();

            foreach (int style_id in style_ids)
            {
                Style style = db.Style.FirstOrDefault(s => s.style_id == style_id);
                m.Styles.Add(style);
            }

            int[] time_ids = db.User_Time
                .Where(ut => ut.user_id == id)
                .Select(us => us.time_id)
                .ToArray();

            foreach(int time_id in time_ids)
            {
                time time = db.time.FirstOrDefault(t => t.id == time_id);
                m.time.Add(time);
            }

            return Ok(m);
        }

        // PUT: api/Musicians/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMusician(int id, Musician musician)
        {
            return Ok();
        }

        // POST: api/Musicians
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostMusician()
        {
            var file = HttpContext.Current.Request.Files["photo"];
            if (file == null || file.ContentLength == 0)
            {
                return BadRequest("No se ha proporcionado ningún archivo.");
            }

            string name = HttpContext.Current.Request.Form["name"];
            string email = HttpContext.Current.Request.Form["email"];
            string password = HttpContext.Current.Request.Form["password"];
            string phone_number = HttpContext.Current.Request.Form["phone_number"];
            string edition_date = HttpContext.Current.Request.Form["edition_date"];
            int municipality_id = int.Parse(HttpContext.Current.Request.Form["province_id"]);
            decimal latitud = decimal.Parse(HttpContext.Current.Request.Form["latitud"]);
            decimal longitud = decimal.Parse(HttpContext.Current.Request.Form["longitud"]);

            var stylesJson = HttpContext.Current.Request.Form["styles"];
            List<Style> styles = JsonConvert.DeserializeObject<List<Style>>(stylesJson);

            var timesJson = HttpContext.Current.Request.Form["times"];
            List<time> times = JsonConvert.DeserializeObject<List<time>>(timesJson);

            if (styles == null || times == null)
            {
                return BadRequest("Los campos 'styles' o 'times' son inválidos.");
            }

            Users usu = new Users();

            usu.name = name;
            usu.photo = "";
            usu.email = email;
            usu.password = password;
            usu.phone_number = phone_number;
            usu.creation_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            usu.edition_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            usu.municipality_id = municipality_id;
            usu.latitud = latitud;
            usu.longitud = longitud;
            db.Users.Add(usu);
            db.SaveChanges();

            string savedFilePath = SaveFile(file, usu.user_id);

            Musician mI = new Musician();
            mI.user_id = usu.user_id;
            db.Musician.Add(mI);
            db.SaveChanges();

            mI.Styles = styles;
            mI.time = times;

            var existingUser = db.Users.FirstOrDefault(u => u.user_id == usu.user_id);

            if (existingUser == null)
            {
                return BadRequest("No se encontró ningún usuario con el ID proporcionado.");
            }

            existingUser.photo = savedFilePath;

            db.Entry(existingUser).State = EntityState.Modified;
            db.SaveChanges();

            foreach (Style s in mI.Styles)
            {
                User_Style user_style = new User_Style { user_id = mI.user_id, style_id = s.style_id};
                db.User_Style.Add(user_style);
            }

            foreach (time t in mI.time)
            {
                User_Time user_time = new User_Time { user_id = mI.user_id, time_id = t.id };
                db.User_Time.Add(user_time);    
            }

            db.SaveChanges();

            return Ok(true);
           
        }

        // DELETE: api/Musicians/5
        [ResponseType(typeof(Musician))]
        public IHttpActionResult DeleteMusician(int id)
        {
            Musician musician = db.Musician.Find(id);
            if (musician == null)
            {
                return NotFound();
            }

            db.Musician.Remove(musician);
            db.SaveChanges();

            return Ok(musician);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MusicianExists(int id)
        {
            return db.Musician.Count(e => e.user_id == id) > 0;
        }
        private bool UsersExists(int id)
        {
            return db.Users.Count(e => e.user_id == id) > 0;
        }

        private string SaveFile(HttpPostedFile file, int userId)
        {
            try
            {
                string relativePath = Path.Combine("Data", "avatar");
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


                string fileName = $"avatar.{userId}{fileExtension}";


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