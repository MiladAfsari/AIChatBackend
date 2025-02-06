namespace Service.Rest.Middlewares
{
    public class RequestResponseData
    {
        public string IP { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public DateTime RequestTimestamp { get; set; }
        public string RequestHeaders { get; set; } = string.Empty;
        public string Authorization { get; set; } = string.Empty;
        public string RequestBody { get; set; } = string.Empty;
        public DateTime ResponseTimestamp { get; set; }
        public string ResponseBody { get; set; } = string.Empty;
        public int ResponseStatusCode { get; set; }
        public int ResponseTime { get; set; }
    }
}
