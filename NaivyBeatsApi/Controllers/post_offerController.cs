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
    public class post_offerController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // POST: api/post_offer
        [ResponseType(typeof(bool))]
        public bool Newpost_offer(post_offer post_offer)
        {
            post_offer po = new post_offer();
            po.creation_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            po.post_id = post_offer.post_id;
            po.user_id = post_offer.user_id;

            db.post_offer.Add(po);
            db.SaveChanges();

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool post_offerExists(int id)
        {
            return db.post_offer.Count(e => e.user_id == id) > 0;
        }
    }
}