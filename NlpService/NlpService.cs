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
        private string executionChannelName = "ExecutionChannel";
        private string nlpChannelName = "NlpChannel";
        private string tridentChannelName = "TridentChannel";
        private Dictionary<Type, Action<IMessage>> typeActionMap;

        public NlpService(IMessageService messageService)
        {
            _messageService = messageService;
            typeActionMap = new Dictionary<Type, Action<IMessage>>();
            typeActionMap.Add(typeof(NlpRequestMessage), HandleNlpRequestMessage);
            typeActionMap.Add(typeof(ExecutionResponseMessage), HandleExecutionResponseMessage);
            InitListeners();
        }

        private void InitListeners()
        {
            // TODO: listen execution response
            _messageService.Listen(nlpChannelName, ChannelType.Direct, OnNlpRequest);
        }

        private void OnNlpRequest(IMessage message)
        {
            try
            {
                typeActionMap[message.GetType()](message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleNlpRequestMessage(IMessage message)
        {
            var request = (NlpRequestMessage) message;

            Console.WriteLine("Got a ! Text=" + request.Text);
            // use SimpleParser or Chatbot to get FFO
            List<Intent> intents = RequestToSimpleParserOrChatbot(request.Text);
            RequestToExecutionEngine(intents, request.RequestUserInfo);
        }

        private void HandleExecutionResponseMessage(IMessage msg)
        {
            var request = (ExecutionResponseMessage) msg;

            Console.WriteLine("Got a exec result message! ResultText=" + request.ResultText + ", RequestUserInfo.UserId: " + request.RequestUserInfo.UserId);

            var message = new NlpResponseMessage();
            message.Response = "NlpService got response from ExecEngine: " + request.ResultText + ", RequestUserInfo.UserId: " + request.RequestUserInfo.UserId;
            message.RequestUserInfo = request.RequestUserInfo;

            _messageService.Publish(tridentChannelName, ChannelType.Direct, message);
        }

        private void RequestToExecutionEngine(List<Intent> intents, UserInfo fromUser)
        {
            var message = new ExecutionRequestMessage();
            //IExecutionObject execObj = ParseIntentToExecutionObject(intent);
            message.Intents = intents;
            message.RequestUserInfo = fromUser;
            _messageService.Publish(executionChannelName, ChannelType.Direct, message);
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
