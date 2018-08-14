using MqService.Attributes;

namespace MqService.Messages.Execution
{
    [DirectMessage]
    public class ExecutionResponseMessage: AbstractMessage
    {
        public string ResultText;
    }
}
