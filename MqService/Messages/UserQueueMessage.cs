using MqService.Attributes;
using MqService.Messages.Contents;

namespace MqService.Messages
{
    /// <summary>
    /// </summary>
    [DirectMessage]
    public class UserQueueMessage: IMessage
    {
        public string UserId;
        public string Text;
        public object Content;
    }
}
