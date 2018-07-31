using System.Collections.Generic;
using MqService.Attributes;
using MqService.Domain;

namespace MqService.Messages
{
    [DirectMessage]
    public class ChatMessage : AbstractMessage
    {
        public List<Intent> Intents;
    }
}
