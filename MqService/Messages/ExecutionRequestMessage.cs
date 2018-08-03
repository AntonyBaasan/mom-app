using System.Collections.Generic;
using MqService.Attributes;
using MqService.Domain;

namespace MqService.Messages
{
    [DirectMessage]
    public class ExecutionRequestMessage
    {
        public List<Intent> Intents;
    }
}
