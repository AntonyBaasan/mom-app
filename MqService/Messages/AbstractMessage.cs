namespace MqService.Messages
{
    public class AbstractMessage : IMessage
    {
        public string Expiration;
        public string GetExpiration() { return Expiration; }
    }
}
