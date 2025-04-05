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
    public class MunicipalitiesController : ApiController
    {
        private NaivyBeatsEntities1 db = new NaivyBeatsEntities1();

        // GET: api/Municipalities
        public IQueryable<Municipality> GetMunicipality()
        {
            return db.Municipality;
        }

        // GET: api/Municipalities/5
        [ResponseType(typeof(Municipality))]
        public IHttpActionResult GetMunicipality(int id)
        {
            Municipality municipality = db.Municipality.Find(id);
            if (municipality == null)
            {
                return NotFound();
            }

            return Ok(municipality);
        }

        // PUT: api/Municipalities/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMunicipality(int id, Municipality municipality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != municipality.municipality_id)
            {
                return BadRequest();
            }

            db.Entry(municipality).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MunicipalityExists(id))
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

        // POST: api/Municipalities
        [ResponseType(typeof(Municipality))]
        public IHttpActionResult PostMunicipality(Municipality municipality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Municipality.Add(municipality);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = municipality.municipality_id }, municipality);
        }

        // DELETE: api/Municipalities/5
        [ResponseType(typeof(Municipality))]
        public IHttpActionResult DeleteMunicipality(int id)
        {
            Municipality municipality = db.Municipality.Find(id);
            if (municipality == null)
            {
                return NotFound();
            }

            db.Municipality.Remove(municipality);
            db.SaveChanges();

            return Ok(municipality);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MunicipalityExists(int id)
        {
            return db.Municipality.Count(e => e.municipality_id == id) > 0;
        }
    }
}