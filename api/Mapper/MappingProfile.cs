using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;
using api.Dtos.User;
using api.Models;
using AutoMapper;

namespace api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Movie -> MovieDto
            CreateMap<Movie, MovieDto>();


            // UserFavorite -> FavoriteDto
            CreateMap<UserFavorite, FavoriteDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Movie, opt => opt.Ignore()); // UserFavorite tablosunda Movie property navigasyon ilişkisi yoksa ignore

            // FavoriteDto -> UserFavorite 
            //Bu dönüşümde sadece minimum veriyi alıp DB’ye yazmak için kullanılır
            CreateMap<FavoriteDto, UserFavorite>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.AppUser, opt => opt.Ignore()); // AppUser navigation property elle set edilmeli



            // UserWatched -> WatchedDto
            CreateMap<UserWatched, WatchedDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Movie, opt => opt.Ignore());

            // WatchedDto -> UserWatched 
            CreateMap<WatchedDto, UserWatched>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.AppUser, opt => opt.Ignore());
        }
    }
}