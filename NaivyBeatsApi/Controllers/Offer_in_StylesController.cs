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
    public class Offer_in_StylesController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();


        // GET: api/Offer_in_Styles/5
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult GetStylesOffer(int id)
        {
            List<string> styles_name = new List<string>();

            int[] styles_ids = db.Offer_in_Styles.Where(os => os.id_offer_in == id)
                                .Select(us => us.style_id)
                                .ToArray();

            foreach (int style_id in styles_ids)
            {
                Style style = db.Style.FirstOrDefault(s => s.style_id == style_id);
                styles_name.Add(style.name);                        
            }

            return Ok(styles_name);
        }
    }
}