using System;
using System.Collections.Generic;
using System.Net.Http;
using MqService;
using MqService.Domain;
using MqService.Messages.Execution;
using RestConsumer;

namespace ExecutionServiceLibrary
{
    public class ExecutionService
    {
        private readonly IMessageService _messageService;
        private RestClient _restClient;
        private string _authToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0L1dpbmRvd3NBdXRoZW50aWNhdGlvblNlcnZpY2UiLCJhdWQiOiI4OTYzMjFtOTY4NTQ3MWtsM2JvYjljODhmMDBqNTY3OCIsImV4cCI6MTU2NTcxMTMzOCwibmJmIjoxNTM0MTc1MzM4fQ.GSf0p3p0fyK-8roT8fu3FRWj3vKrHKN2u7gDhamMe6c";
        private string _xsrfToken = "HKN+9JVUv1iGwAWfcWYNTvkFGSqK4pUWiyC/qmGHdSsiVoefHGVTwIFxRTs84wB1+0d6WCJIvm6prbT+HVzYAw==";

        public ExecutionService(IMessageService messageService)
        {
            _messageService = messageService;
            _messageService.ListenMessage<ExecutionRequestMessage>(OnExecutionRequestReceived);

            _restClient = new RestClient("http://localhost/PROPHIX/");
        }

        public void OnExecutionRequestReceived(ExecutionRequestMessage request)
        {
            try
            {
                List<Intent> list = request.Intents;
                Console.WriteLine($"Got a chat message with {list.Count} intenst(s)!");

                var httpResponse = SendHttpRequest();
                Console.WriteLine($"Called and received message! httpResponse.StatusCode: " + httpResponse.StatusCode);

                var message = new ExecutionResponseMessage();
                message.ResultText = "ExecutedObject1";
                message.RequestUserInfo = request.RequestUserInfo;
                _messageService.Publish(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private HttpResponseMessage SendHttpRequest()
        {
            var param = new Dictionary<string, object>();
            param.Add("fileID", 6145);
            var responseMessage = _restClient.GetAsync(new RequestArgs()
            {
                Uri = "/DocEx/GetFileInfo",
                AuthToken = _authToken,
                XsrfToken = _xsrfToken
            }, param);

            return responseMessage.Result;
        }
    }
}
