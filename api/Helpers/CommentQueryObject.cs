using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class CommentQueryObject
    {
        //public string? ImdbID { get; set; }  //???????????????
        public int? MovieId { get; set; }
        public string? Title { get; set; }
        public bool IsDecsending { get; set; } = true; //yeniden eskiye göre sıralancak 
    }
}