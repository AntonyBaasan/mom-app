using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqService;
using MqService.Domain;
using MqService.Messages;

namespace ExecutionServiceLibrary
{
    public class ExecutionService
    {
        private readonly IMessageService _messageService;

        public ExecutionService(IMessageService messageService)
        {
            _messageService = messageService;
            _messageService.ListenMessage<ChatMessage>(OnChatReceived);
        }

        public void OnChatReceived(ChatMessage msg)
        {
            List<Intent> list = msg.Intents;

            Console.WriteLine($"Got a chat message with {list.Count} intenst(s)!");
            var message = new ExecutionMessage();
            message.ResultText = "ExecutedObject1";
            _messageService.Publish(message);
        }
    }
}
