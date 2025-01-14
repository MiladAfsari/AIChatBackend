namespace Domain.Core.Entities.InvalidatedTokenTemplateAggregate
{
    public class InvalidatedToken
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string Token { get; set; }
        public DateTime InvalidatedAt { get; set; }
    }
}
