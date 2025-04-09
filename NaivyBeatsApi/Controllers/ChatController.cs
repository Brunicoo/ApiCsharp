using NaivyBeatsApi.Models;
using System;
using System.Collections.Generic;
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

        // GET: api/Chat/2
        [Route("api/Chat/{user_id}")]
        [ResponseType(typeof(List<Chat>))]
        public List<Chat> getChatByUserId(int user_id)
        {
            List<Chat> chats = db.Chat.Where(ch => ch.musician_id == user_id || ch.restaurant_id == user_id).ToList();

            return chats;
        }

    }
}