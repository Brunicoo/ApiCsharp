using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Linq;
using NaivyBeatsApi.Models;

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
        public IHttpActionResult PostMusician(Musician musician)
        {
            
            Users usu = new Users();
        

            usu.name = musician.name;
            usu.photo = musician.photo;
            usu.email = musician.email;
            usu.password = musician.password;
            usu.phone_number = musician.phone_number;
            usu.creation_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            usu.edition_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            usu.municipality_id = musician.municipality_id;
            usu.latitud = musician.latitud;
            usu.longitud = musician.longitud;
            db.Users.Add(usu);

            Musician mI = new Musician { user_id = musician.user_id};
            db.Musician.Add(mI);

            foreach (Style s in musician.Styles)
            {
                User_Style user_style = new User_Style { user_id = musician.user_id, style_id = s.style_id};
                db.User_Style.Add(user_style);
            }

            foreach (time t in musician.time)
            {
                User_Time user_time = new User_Time { user_id = musician.user_id, time_id = t.id };
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
    }
 }