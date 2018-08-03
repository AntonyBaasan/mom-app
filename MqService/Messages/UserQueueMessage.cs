using MqService.Attributes;

namespace MqService.Messages
{
    /// <summary>
    /// </summary>
    [DirectMessage]
    public class UserQueueMessage: IMessage
    {
        public string UserId;
        public object Content;
    }
}
