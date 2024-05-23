namespace map_tile_server.Models.Details
{
    public class RoutingDetail
    {
        public string Type { get; set; } = string.Empty;
        public int[][][]? Coordinates { get; set; } 
    }
}
