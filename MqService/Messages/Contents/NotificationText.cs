namespace MqService.Messages.Contents
{
    public class NotificationText
    {
        public MessageContentType ContentType { get { return MessageContentType.NotificationText; } }
        public string Text;
    }
}
