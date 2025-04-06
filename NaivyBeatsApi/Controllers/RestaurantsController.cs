using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using NaivyBeatsApi.Models;
using Newtonsoft.Json;

namespace NaivyBeatsApi.Controllers
{
    public class RestaurantsController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // GET: api/Restaurants
        public IQueryable<Restaurant> GetRestaurant()
        {
            return db.Restaurant;
        }

        // GET: api/Restaurants/5
        [ResponseType(typeof(Restaurant))]
        public IHttpActionResult GetRestaurant(int id)
        {
            Users user = db.Users.Find(id);

            Restaurant r = db.Restaurant.Find(user.user_id);
            r.name = user.name;
            r.photo = user.photo;
            r.email = user.email;
            r.password = user.password;
            r.phone_number = user.phone_number;
            r.creation_date = user.creation_date;
            r.edition_date = user.edition_date;
            r.deleted_at = user.deleted_at;
            r.municipality_id = user.municipality_id;
            r.latitud = user.latitud;
            r.longitud = user.longitud;

            return Ok(r);

        }

        // PUT: api/Restaurants/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurant(int id, Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurant.user_id)
            {
                return BadRequest();
            }

            db.Entry(restaurant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Restaurants
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostRestaurant()
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
            String latitud = HttpContext.Current.Request.Form["latitud"];
            String longitud = HttpContext.Current.Request.Form["longitud"];
            string opening_time = HttpContext.Current.Request.Form["opening_time"];
            string closing_time = HttpContext.Current.Request.Form["closing_time"];

            var latitudD = decimal.Parse(latitud, CultureInfo.InvariantCulture);
            var longitudD = decimal.Parse(longitud, CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            if (municipality_id <= 0)
            {
                return BadRequest("El ID de municipio debe ser mayor que cero.");
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
            usu.latitud = latitudD;
            usu.longitud = longitudD;
            db.Users.Add(usu);
            db.SaveChanges();

            string savedFilePath = SaveFile(file, usu.user_id);

            Restaurant restaurant = new Restaurant();
            restaurant.user_id = usu.user_id;
            restaurant.opening_time = opening_time;
            restaurant.closing_time = closing_time;
            db.Restaurant.Add(restaurant);
            db.SaveChanges();

  

            var existingUser = db.Users.FirstOrDefault(u => u.user_id == usu.user_id);

            if (existingUser == null)
            {
                return BadRequest("No se encontró ningún usuario con el ID proporcionado.");
            }

            existingUser.photo = savedFilePath;

            db.Entry(existingUser).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(true);

        }

        // DELETE: api/Restaurants/5
        [ResponseType(typeof(Restaurant))]
        public IHttpActionResult DeleteRestaurant(int id)
        {
            Restaurant restaurant = db.Restaurant.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            db.Restaurant.Remove(restaurant);
            db.SaveChanges();

            return Ok(restaurant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantExists(int id)
        {
            return db.Restaurant.Count(e => e.user_id == id) > 0;
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