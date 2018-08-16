namespace MqService.Messages
{
    public interface IMessage
    {
        MessageMetadata Metadata { get; set; }
    }
}
