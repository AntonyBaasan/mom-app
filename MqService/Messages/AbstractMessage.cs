namespace MqService.Messages
{
    public class AbstractMessage : IMessage
    {
        public MessageMetadata Metadata { get; set; }
    }
}
