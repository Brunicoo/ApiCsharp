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
    public class StylesController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // GET: api/Styles
        public IQueryable<Style> GetStyle()
        {
            return db.Style;
        }

        // GET: api/Styles/5
        [ResponseType(typeof(Style))]
        public IHttpActionResult GetStyle(int id)
        {
            Style style = db.Style.Find(id);
            if (style == null)
            {
                return NotFound();
            }

            return Ok(style);
        }

        // PUT: api/Styles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStyle(int id, Style style)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != style.style_id)
            {
                return BadRequest();
            }

            db.Entry(style).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StyleExists(id))
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

        // POST: api/Styles
        [ResponseType(typeof(Style))]
        public IHttpActionResult PostStyle(Style style)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Style.Add(style);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = style.style_id }, style);
        }

        // DELETE: api/Styles/5
        [ResponseType(typeof(Style))]
        public IHttpActionResult DeleteStyle(int id)
        {
            Style style = db.Style.Find(id);
            if (style == null)
            {
                return NotFound();
            }

            db.Style.Remove(style);
            db.SaveChanges();

            return Ok(style);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StyleExists(int id)
        {
            return db.Style.Count(e => e.style_id == id) > 0;
        }
    }
}