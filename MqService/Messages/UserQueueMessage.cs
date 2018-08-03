using MqService.Attributes;
using MqService.Messages.Contents;

namespace MqService.Messages
{
    /// <summary>
    /// Route is required because notification module should listen 
    /// individual user channed. User ID will be set as route name
    /// </summary>
    [DirectMessage]
    public class UserQueueMessage: IMessage
    {
        public string UserId;
        public string Text;
        public object Content;
    }
}
