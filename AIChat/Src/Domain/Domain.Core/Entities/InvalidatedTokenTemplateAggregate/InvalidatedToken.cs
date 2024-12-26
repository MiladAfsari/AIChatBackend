using Domain.Base;

namespace Domain.Core.Entities.InvalidatedTokenTemplateAggregate
{
    public class InvalidatedToken 
    {
        public Guid ID { get; set; }
        public string Token { get; set; }
        public DateTime InvalidatedAt { get; set; }
    }
}
