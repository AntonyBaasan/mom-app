using MqService.Attributes;

namespace MqService.Messages
{
    /// <summary>
    /// Route is required because notification module should listen 
    /// individual user channed. User ID will be set as route name
    /// </summary>
    [BroadcastMessage(RouteRequired = true, Target = BroadcastTarget.Application)]
    public class UserNotificationMessage: IMessage
    {
        public string UserId;
        public string Text;
    }
}
