using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }

        public static Result<T> SuccessResult(T data) => new() { Success = true, Data = data };
        public static Result<T> FailResult(string error) => new() { Success = false, Error = error };
    }
}