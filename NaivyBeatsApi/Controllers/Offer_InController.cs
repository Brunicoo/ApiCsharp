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
    public class Offer_InController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // GET: api/Offer_In
        public List<Offer_In> GetOffer_In()
        {
            List<Offer_In> offers_In = db.Offer_In.Where(of => of.music_id_final == null).ToList();

            foreach (Offer_In of in offers_In)
            {
                of.styles_ids = db.Offer_in_Styles.Where(ois => ois.id_offer_in == of.offer_in_id)
                                                    .Select(ois => ois.style_id)
                                                    .ToList();
            }

            return offers_In;
        }

        // POST: api/Offer_In
        [ResponseType(typeof(bool))]
        public bool PostOffer_In(Offer_In offer_In)
        {

            Offer_In of = new Offer_In();
            of.event_date = offer_In.event_date;
            of.publish_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            of.restaurant_id = offer_In.restaurant_id;
            of.description = offer_In.description;
            of.salary = offer_In.salary;
            of.done = 0;

            db.Offer_In.Add(of);
            db.SaveChanges();

            of.styles_ids = offer_In.styles_ids;    

            foreach (int style_id in of.styles_ids)
            {
                Offer_in_Styles ois = new Offer_in_Styles();
                ois.id_offer_in = of.offer_in_id;
                ois.style_id = style_id;

                db.Offer_in_Styles.Add(ois);
            }
            db.SaveChanges();

           return true;
        }

        // GET: api/Offer_In/5
        [ResponseType(typeof(Offer_In))]
        public IHttpActionResult GetOffer_In(int id)
        {
            Offer_In offer_In = db.Offer_In.Find(id);
            if (offer_In == null)
            {
                return NotFound();
            }

            return Ok(offer_In);
        }

        // PUT: api/Offer_In/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOffer_In(int id, Offer_In offer_In)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != offer_In.offer_in_id)
            {
                return BadRequest();
            }

            db.Entry(offer_In).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Offer_InExists(id))
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

        // DELETE: api/Offer_In/5
        [ResponseType(typeof(Offer_In))]
        public IHttpActionResult DeleteOffer_In(int id)
        {
            Offer_In offer_In = db.Offer_In.Find(id);
            if (offer_In == null)
            {
                return NotFound();
            }

            db.Offer_In.Remove(offer_In);
            db.SaveChanges();

            return Ok(offer_In);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Offer_InExists(int id)
        {
            return db.Offer_In.Count(e => e.offer_in_id == id) > 0;
        }
    }
}