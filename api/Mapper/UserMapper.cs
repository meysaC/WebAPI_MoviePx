using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Models;

namespace api.Mapper
{
    public static class UserMapper
    {
        public static FavoriteDto? UserPreferanceToFavoriteDto(this UserPreferance preferance) 
        {
            return new FavoriteDto
            {
                Id = preferance.Id,
                Favorite = "Favorite", 
                Title = "",            
                Director = "",         
                imdbRating = ""       
            };
        }
        public static UserPreferance ToUserPreferanceFromFavoriteDto(this FavoriteDto favoriteDto)
        {
            return new UserPreferance
            {
                IsFavorite = true,
                CreatedAt = DateTime.Now // bunu brda yapma!!!!!!!!!!!
            };
        }
        
        
        // public static List<FavoriteDto?> FavoritesDtoToFavoriteDto(this FavoritesDto favoriteDto) //Gereksiz!!!!!
        // {
        //     return favoriteDto.Favorites.Select(favorites => new FavoriteDto
        //     {
        //         Title = favorites.Title,
        //         Director = favorites.Director,
        //         imdbRating = favorites.imdbRating
        //     }).ToList();
        // }

        public static UserFollow? ToUserFollowFromFollowDto(this FollowDto followDto)
        {
            return new UserFollow
            {
                Id = followDto.Id
            };
        }
        public static FollowDto?  UserFollowToFollowDto(this UserFollow followModel)
        {
            return new FollowDto
            {
                Id = followModel.Id,
                userName = followModel.AppUser.UserName,
                FollowingUserName = "",
                Since = ""
            };
        }


    }
}