namespace map_tile_server.Models.Commons
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

    public class ListSuccessDetail<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; }
        public int Total { get; set; }

        public ListSuccessDetail(List<T> data, int total)
        {
            Success = true;
            Data = data;
            Total = total;
        }
    }
}
