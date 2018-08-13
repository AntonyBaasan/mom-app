namespace MqService.Messages
{
    public interface IMessage
    {
        string GetExpiration();
        UserInfo GetRequestUserInfo();
    }
}
