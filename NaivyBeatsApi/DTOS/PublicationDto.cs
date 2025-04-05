using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace NaivyBeatsApi.Models
{
    public partial class PublicationDto
    {
        public int publication_id { get; set; }
        public string publication_date { get; set; }
        public string description { get; set; }
        public HttpPostedFile multimedia_content { get; set; }
        public int user_id { get; set; }
        public string titulo { get; set; }
    }
}
