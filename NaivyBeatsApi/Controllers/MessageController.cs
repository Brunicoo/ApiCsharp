using NaivyBeatsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace NaivyBeatsApi.Controllers
{
    public class MessageController : ApiController
    {
        private NaivyBeatsEntities db = new NaivyBeatsEntities();

        // POST: api/Message
        [ResponseType(typeof(bool))]
        public bool newMessage(Message message)
        {
            Message m = new Message();

            m.text = message.text;
            m.publish_date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            m.chat_id = message.chat_id;
            m.user_id = message.user_id;

            db.Message.Add(m);
            db.SaveChanges();

            return true;
        }

        // GET: api/Message/2
        [Route("api/Message/{chat_id}")]
        [ResponseType(typeof(List<Message>))]
        public List<Message> getMessagesByChatId(int chat_id)
        {
            List<Message> messages = db.Message.Where(m => m.chat_id == chat_id).ToList();

            return messages;
        }
    }
}