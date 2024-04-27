﻿namespace map_tile_server.Models.Commons
{
    public class SuccessDetail<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }

        public SuccessDetail(T data)
        {
            Success = true;
            Data = data;
        }
    }

    public class LoginSuccessDetail
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public LoginSuccessDetail(string token)
        {
            Success = true;
            Token = token;
        }
    }
}