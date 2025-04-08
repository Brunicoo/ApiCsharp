using NaivyBeatsApi.Models;
using System;
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
    }
}