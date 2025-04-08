using NaivyBeatsApi.Models;
using System;
using System.Linq;
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

        // GET: api/Chat/2/3
        [Route("api/Chat/{musician_id}/{restaurant_id}")]
        [ResponseType(typeof(Chat))]
        public Chat getChatByMusicianAndRestaurantId(int musician_id, int restaurant_id)
        {
            Chat c = db.Chat.FirstOrDefault(ch => ch.musician_id == musician_id && ch.restaurant_id == restaurant_id);

            return c;
        }
    }
}