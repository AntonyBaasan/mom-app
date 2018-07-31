using MqService.Attributes;

namespace MqService.Messages
{
    [DirectMessage]
    public class ExecutionMessage: AbstractMessage
    {
        public string ResultText;
    }
}
