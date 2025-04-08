using NaivyBeatsApi.Models;
using System;
using System.Web.Http;
using System.Web.Http.Description;

namespace NaivyBeatsApi.Controllers
{
    public class ChatController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();


        // POST: api/Chat
        [ResponseType(typeof(bool))]
        public bool newChat(Chat chat)
        {
            Chat c = new Chat();
            
            c.creation_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            c.musician_id = chat.musician_id;
            c.restaurant_id = chat.restaurant_id;

            db.Chat.Add(c);
            db.SaveChanges();

            return true;
        }
    }
}