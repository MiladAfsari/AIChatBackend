using System.ComponentModel.DataAnnotations;

namespace Domain.Core.ApiLog
{
    public class ApiLog
    {
        [Key]
        public long Id { get; set; }
        public string? ControllerName { get; set; }
        public string? RequestHeaders { get; set; }
        public string? AccessToken { get; set; }
        public string? MethodInput { get; set; }
        public string? MethodOutput { get; set; }
        public int ResultCode { get; set; }
        public string? ResultMessage { get; set; }
        public DateTime DateTime { get; set; }
        public string? IP { get; set; }
        public double ResponseTime { get; set; }
    }
}
