using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.UserRating;
using api.Models;

namespace api.Mapper
{
    public static class UserRatingMapper
    {
        public static UserRatingDto? UserRatingToUserRatingDto(this UserRating ratingModel)
        {
            return new UserRatingDto
            {
                Id = ratingModel.Id,
                Rate = ratingModel.Rate,
                ImdbID = ratingModel.ImdbID,
                CreatedBy = ratingModel.AppUser.UserName
            };
        }

        // public static UserRating UserRatingDtoToUserRating(this UserRatingDto ratingDto)
        // {
        //     return new UserRating
        //     {
        //         Id = ratingDto.Id,
        //         Rate = ratingDto.Rate,
        //         ImdbID = ratingDto.ImdbID,
        //         //AppUserId = ratingDto.CreatedBy
        //     };
        // }

        public static MovieUserRatingDto? UserRatingToMovieRatingRatioDto(string imdbID, double ratio) //this UserRating ratingModel
        {
            return new MovieUserRatingDto
            {
                ImdbID = imdbID,
                RatingRatio = ratio
            };
        }

        public static UserRating? CreateUserRatingDtoToUserRating(this CreateUserRatingDto ratingDto)
        {
            return new UserRating
            {
                ImdbID = ratingDto.ImdbID,
                Rate = ratingDto.Rate
            };
        }
    }
}