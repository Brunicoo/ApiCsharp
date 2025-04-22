using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NaivyBeatsApi.Models;

namespace NaivyBeatsApi.DTOS
{
    public class PostLike: Publication
    {
        public int like;
        public int chat;
        public int follow;
    }
}