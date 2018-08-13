using MqService.Attributes;

namespace MqService.Messages
{
    [DirectMessage]
    public class NotificationMessage : AbstractMessage
    {
        public string[] To { get; set; }
        public string Text { get; set; }
    }
}
