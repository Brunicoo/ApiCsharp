using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NaivyBeatsApi.Models;

namespace NaivyBeatsApi.Controllers
{
    public class timesController : ApiController
    {
        private NaivyBeatsEntities1 db = new NaivyBeatsEntities1();

        // GET: api/times
        public IQueryable<time> Gettime()
        {
            return db.time;
        }

        // GET: api/times/5
        [ResponseType(typeof(time))]
        public IHttpActionResult Gettime(int id)
        {
            time time = db.time.Find(id);
            if (time == null)
            {
                return NotFound();
            }

            return Ok(time);
        }

        // PUT: api/times/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Puttime(int id, time time)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != time.id)
            {
                return BadRequest();
            }

            db.Entry(time).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!timeExists(id))
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

        // POST: api/times
        [ResponseType(typeof(time))]
        public IHttpActionResult Posttime(time time)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.time.Add(time);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = time.id }, time);
        }

        // DELETE: api/times/5
        [ResponseType(typeof(time))]
        public IHttpActionResult Deletetime(int id)
        {
            time time = db.time.Find(id);
            if (time == null)
            {
                return NotFound();
            }

            db.time.Remove(time);
            db.SaveChanges();

            return Ok(time);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool timeExists(int id)
        {
            return db.time.Count(e => e.id == id) > 0;
        }
    }
}