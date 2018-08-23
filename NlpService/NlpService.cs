using System;
using System.Collections.Generic;
using ExecutionService;
using ExecutionService.Commands;
using MqService;
using MqService.Domain;
using MqService.Helper;
using MqService.Messages;
using MqService.Messages.Execution;
using MqService.Messages.Nlp;

namespace NlpLibrary
{
    public class NlpService
    {
        private readonly IMessageService _messageService;
        private readonly IExecutionService _executionService;
        private Dictionary<Type, Action<IMessage>> typeActionMap;

        public NlpService(IMessageService messageService, IExecutionService executionService)
        {
            _messageService = messageService;
            _executionService = executionService;
            InitTypeActionMap();
            InitListeners();
        }

        private void InitTypeActionMap()
        {
            typeActionMap = new Dictionary<Type, Action<IMessage>>();
            typeActionMap.Add(typeof(NlpRequestMessage), HandleNlpRequestMessage);
            //typeActionMap.Add(typeof(ExecutionResponseMessage), HandleExecutionResponseMessage);
        }

        private void InitListeners()
        {
            _messageService.Listen(Channels.NLP, ChannelType.Direct, OnNlpRequest);
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
            Intent intent = RequestToSimpleParserOrChatbot(request.Text);
            //RequestToExecutionEngine(intents, request.Metadata.RequestUserInfo);

            ICommand command = ConvertIntentToCommand(intent);
            _executionService.Execute(command, request.Metadata.RequestUserInfo);
        }

        private ICommand ConvertIntentToCommand(Intent intent)
        {
            throw new NotImplementedException();
        }

        //private void HandleExecutionResponseMessage(IMessage msg)
        //{
        //    var request = (ExecutionResponseMessage) msg;

        //    var requestUserInfo = request.Metadata.RequestUserInfo;
        //    Console.WriteLine("Got a exec result message! ResultText=" + request.ResultText + ", RequestUserInfo.UserId: " + requestUserInfo.UserId);

        //    var message = new NlpResponseMessage();
        //    message.Response = "NlpService got response from ExecEngine: " + request.ResultText + ", RequestUserInfo.UserId: " + requestUserInfo.UserId;
        //    message.Metadata = new MessageMetadata() { RequestUserInfo = requestUserInfo };

        //    _messageService.Send(Channels.TRIDENT_USER, ChannelType.Direct, message);
        //}

        //private void RequestToExecutionEngine(List<Intent> intents, UserInfo fromUser)
        //{
        //    var message = new ExecutionRequestMessage();
        //    //IExecutionObject execObj = ParseIntentToExecutionObject(intent);
        //    message.Intents = intents;
        //    message.Metadata = new MessageMetadata() { RequestUserInfo = fromUser };
        //    _messageService.Send(Channels.EXECUTION, ChannelType.Direct, message);
        //}

        //private IExecutionObject ParseIntentToExecutionObject(Intent intent)
        //{
        //    throw new NotImplementedException();
        //}

        //private List<Intent> RequestToSimpleParserOrChatbot(string text)
        //{
        //    List<Intent> list = new List<Intent>();
        //    list.Add(new Intent { Name = "OpenFile" });
        //    list.Add(new Intent { Name = "OpenFile" });

        //    return list;
        //}

        private Intent RequestToSimpleParserOrChatbot(string text)
        {
            return new Intent { Name = "OpenFile" };
        }

        public void SendAudio(string text)
        {

        }
    }
}
