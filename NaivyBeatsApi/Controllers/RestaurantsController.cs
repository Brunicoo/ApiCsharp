using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NaivyBeatsApi.Models;

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
        public IHttpActionResult PostRestaurant(Restaurant restaurant)
        {
            Users usu = new Users();

            usu.user_id = 0;
            usu.name = restaurant.name;
            usu.photo = restaurant.photo;
            usu.email = restaurant.email;
            usu.password = restaurant.password;
            usu.phone_number = restaurant.phone_number;
            usu.creation_date = DateTime.Now.Date;
            usu.edition_date = DateTime.Now.Date;
            usu.municipality_id = restaurant.municipality_id;
            usu.latitud = restaurant.latitud;
            usu.longitud = restaurant.longitud;
            db.Users.Add(usu);
            db.SaveChanges();

            Restaurant r = new Restaurant();
            r.user_id = usu.user_id;
            r.closing_time = restaurant.closing_time;
            r.opening_time = restaurant.opening_time;

            db.Restaurant.Add(r);
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
    }
}