using MqService.Attributes;

namespace MqService.Messages
{
    [BroadcastMessage(Target = BroadcastTarget.All)]
    public class SystemControllerMessage: IMessage
    {
    }
}
