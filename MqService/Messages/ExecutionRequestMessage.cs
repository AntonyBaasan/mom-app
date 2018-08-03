using System.Collections.Generic;
using MqService.Attributes;
using MqService.Domain;

namespace MqService.Messages
{
    [DirectMessage]
    public class ExecutionRequestMessage : IMessage
    {
        public List<Intent> Intents;
    }
}
