using System;
using System.Collections.Generic;
using MqService;
using MqService.Domain;
using MqService.Messages.Execution;

namespace ExecutionServiceLibrary
{
    public class ExecutionService
    {
        private readonly IMessageService _messageService;

        public ExecutionService(IMessageService messageService)
        {
            _messageService = messageService;
            _messageService.ListenMessage<ExecutionRequestMessage>(OnExecutionRequestReceived);
        }

        public void OnExecutionRequestReceived(ExecutionRequestMessage msg)
        {
            List<Intent> list = msg.Intents;

            Console.WriteLine($"Got a chat message with {list.Count} intenst(s)!");
            var message = new ExecutionResponseMessage();
            message.ResultText = "ExecutedObject1";
            message.From = msg.From;
            _messageService.Publish(message);
        }
    }
}
