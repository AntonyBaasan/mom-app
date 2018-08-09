using System.Collections.Generic;
using MqService.Attributes;
using MqService.Domain;

namespace MqService.Messages.Execution
{
    [DirectMessage(Expiration = "")]
    public class ExecutionRequestMessage : AbstractMessage
    {
        public List<Intent> Intents;
        public string From;
    }
}
