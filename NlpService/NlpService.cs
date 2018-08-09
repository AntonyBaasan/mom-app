using System;
using System.Collections.Generic;
using MqService;
using MqService.Domain;
using MqService.Messages;
using MqService.Messages.Execution;
using MqService.Messages.Nlp;

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
            _messageService.ListenMessage<NlpRequestMessage>(OnNlpRequest);
            _messageService.ListenMessage<ExecutionResponseMessage>(OnExecutionResponse);
        }

        private void OnNlpRequest(NlpRequestMessage msg)
        {
            Console.WriteLine("Got a NlpRequestMessage! Text=" + msg.Text);
            // use SimpleParser or Chatbot to get FFO
            List<Intent> intents = RequestToSimpleParserOrChatbot(msg.Text);
            RequestToExecutionEngine(intents, msg.From);
        }

        private void OnExecutionResponse(ExecutionResponseMessage msg)
        {
            Console.WriteLine("Got a exec result message! ResultText=" + msg.ResultText + ", From: " + msg.From);

            var message = new NlpResponseMessage();
            message.Response = "NlpService got response from ExecEngine: " + msg.ResultText + ", From: " + msg.From;
            message.To = msg.From;

            _messageService.Publish(message);
        }

        private void RequestToExecutionEngine(List<Intent> intents, string @from)
        {
            var message = new ExecutionRequestMessage();
            //IExecutionObject execObj = ParseIntentToExecutionObject(intent);
            message.Intents = intents;
            message.From = from;
            _messageService.Publish(message);
        }


        //private IExecutionObject ParseIntentToExecutionObject(Intent intent)
        //{
        //    throw new NotImplementedException();
        //}

        private List<Intent> RequestToSimpleParserOrChatbot(string text)
        {
            List<Intent> list = new List<Intent>();
            list.Add(new Intent { Name = "OpenFile" });
            list.Add(new Intent { Name = "OpenFile" });

            return list;
        }

        public void SendAudio(string text)
        {

        }
    }
}
