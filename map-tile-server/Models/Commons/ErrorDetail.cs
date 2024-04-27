using System.Text.Json;

namespace map_tile_server.Models.Commons
{
    public class ErrorDetail
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public ErrorDetail(int status, string message)
        {
            StatusCode = status;
            Message = message;
        }
    }
}
