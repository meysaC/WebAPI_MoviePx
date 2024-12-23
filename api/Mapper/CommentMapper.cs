using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Dtos.Comment;
using Microsoft.AspNetCore.Components.Web;

namespace api.Mapper
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment commentModel) //Bir metodu extension method yapmak için this anahtar kelimesini eklemek gereklidir. (extension metotlar static dir!)
                                                                    //Aksi takdirde, metot sadece bir statik metot olarak çalışır ve yalnızca sınıf adıyla (CommentMapper.ToCommentDto(...)) çağrılabilir.
                                                                    //Extension method, mevcut bir sınıfın veya arabirimin (örneğin, Comment) değiştirilmesine gerek kalmadan o sınıfa yeni bir metot eklemenizi sağlar
                                                                    //İlk parametresinde this anahtar kelimesini kullanarak, genişletilecek sınıfı belirtir.
                                                                    //Özellikle LINQ ile çalışırken (ör. Select, Where gibi), extension metotlar doğal bir kullanım sunar.
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                //ImdbID = commentModel.ImdbID,
                CreatedBy = commentModel.AppUser.UserName
            };
        }

        public static Comment ToCommentFromCreateDto(this CommentCreateDto createCommentDto, string ImdbID)
        {
            return new Comment
            {
                Title = createCommentDto.Title,
                Content = createCommentDto.Content,
                ImdbID = ImdbID
            };
        }

        public static Comment ToCommentFromUpdateDto(this CommentUpdateDto commentDto)
        {
            if (commentDto == null)
            throw new ArgumentNullException(nameof(commentDto), "Comment model is null.");

            return new Comment
            {
                Title = commentDto.Title ?? "Default Title", 
                Content = commentDto.Content ?? "Default Content" 
            };

        //     return new Comment
        //     {
        //         Title = commentDto.Title ?? null ,
        //         Content = commentDto.Content
        //     };
        }

    }
}