using System;
using System.Collections.Generic;
using MqService;
using MqService.Domain;
using MqService.Messages;

namespace NlpLibrary
{
    public class NlpService
    {
        private readonly IMessageService _messageService;

        public NlpService(IMessageService messageService)
        {
            _messageService = messageService;

            InitListeners();
        }

        private void InitListeners()
        {
            // TODO: listen execution response
            _messageService.ListenMessage<ExecutionMessage>(OnExecutionResponse);
        }

        private void OnExecutionResponse(ExecutionMessage msg)
        {
            Console.WriteLine("Got a exec result message! ResultText=" + msg.ResultText);
        }

        public void SendText(string text)
        {
            //TODO: use SimpleParser or Chatbot to get FFO
            Intent intent = SendRequestToSimpleParserOrChatbot(text);
            List<Intent> list = new List<Intent>();
            list.Add(intent);
            list.Add(intent);

            //IExecutionObject execObj = ParseIntentToExecutionObject(intent);

            var message = new ChatMessage();
            message.Intents = list;
            _messageService.Publish(message);
        }

        //private IExecutionObject ParseIntentToExecutionObject(Intent intent)
        //{
        //    throw new NotImplementedException();
        //}

        private Intent SendRequestToSimpleParserOrChatbot(string text)
        {
            return new Intent { Name = "OpenFile" };
        }

        public void SendAudio(string text)
        {

        }
    }
}
