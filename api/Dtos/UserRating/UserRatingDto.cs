using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.UserRating
{
    public class UserRatingDto
    {
        public int Id { get; set; }       
        public double Rate { get; set; }
        public string ImdbID { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}