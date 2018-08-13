namespace MqService.Messages
{
    public class AbstractMessage : IMessage
    {
        public string Expiration;
        public string GetExpiration() { return Expiration; }
        public UserInfo RequestUserInfo { get; set; }
        public UserInfo GetRequestUserInfo() { return RequestUserInfo; }
    }
}
