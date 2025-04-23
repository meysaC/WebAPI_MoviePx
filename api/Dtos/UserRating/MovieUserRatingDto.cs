using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.UserRating
{
    public class MovieUserRatingDto
    {
        public int MovieId { get; set; }
        public double RatingRatio { get; set; }
    }
}