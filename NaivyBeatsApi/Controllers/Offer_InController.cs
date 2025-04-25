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
using NaivyBeatsApi.DTOS;
using NaivyBeatsApi.Models;

namespace NaivyBeatsApi.Controllers
{
    public class Offer_InController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // GET: api/Offer_In/2
        [Route("api/Offer_In/{user_id}")]
        public List<OfferInDto> GetOffer_In(int user_id)
        {
            List<Offer_In> offers_In = db.Offer_In.Where(of => of.music_id_final == null).ToList();

            foreach (Offer_In of in offers_In)
            {
                of.styles_ids = db.Offer_in_Styles.Where(ois => ois.id_offer_in == of.offer_in_id)
                                                    .Select(ois => ois.style_id)
                                                    .ToList();
            }

            List<OfferInDto> offersDto = new List<OfferInDto>();
            foreach (var offer in offers_In)
            {
                var exists = db.post_offer.FirstOrDefault(pf => pf.post_id == offer.offer_in_id && pf.user_id == user_id);

                OfferInDto offDto = new OfferInDto();
                offDto.done = offer.done;
                offDto.restaurant_id = offer.restaurant_id;
                offDto.event_date = offer.event_date;
                offDto.publish_date = offer.publish_date;
                offDto.description = offer.description;
                offDto.styles_ids = offer.styles_ids;
                offDto.music_id_final = offer.music_id_final;
                offDto.salary = offer.salary;
                offDto.offer_in_id = offer.offer_in_id;

                if (exists != null)
                {
                    offDto.postulated = 1;
                } else
                {
                    offDto.postulated = 0;
                }  

                offersDto.Add(offDto);
            }  

            return offersDto;
        }

        // POST: api/Offer_In
        [ResponseType(typeof(int))]
        public int PostOffer_In(Offer_In offer_In)
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

           return of.offer_in_id;
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