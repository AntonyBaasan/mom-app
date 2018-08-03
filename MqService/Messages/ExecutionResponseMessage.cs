using MqService.Attributes;

namespace MqService.Messages
{
    [DirectMessage]
    public class ExecutionResponseMessage: AbstractMessage
    {
        public string ResultText;
    }
}
