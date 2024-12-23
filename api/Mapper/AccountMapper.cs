using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Helpers;
using api.Models;

namespace api.Mapper
{
    public static class AccountMapper
    {
        public static FavoriteDto? UserPreferanceToFavoriteDto(this UserPreferance preferance) //
        {
            return new FavoriteDto
            {
                Id = preferance.Id,
                Favorite = "Favorite", 
                Title = "",            
                Director = "",         
                imdbRating = "",       
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
    }
}