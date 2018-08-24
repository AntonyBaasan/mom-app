using System;
using System.Collections.Generic;
using ExecutionService.Commands;
using MqService;
using MqService.Messages;
using RestConsumer;

namespace ExecutionService
{
    public class ExecutionService : IExecutionService
    {
        private readonly IMessageService _messageService;
        private readonly IRestClient _restClient;

        public ExecutionService(IMessageService messageService, IRestClient restClient)
        {
            _messageService = messageService;
            _restClient = restClient;
        }
        public object Execute(ICommand command, MessageMetadata metadata)
        {
            var executionContext = new ExecutionContext()
            {
                messageService = _messageService,
                restClient = _restClient,
                metaData = metadata
            };
            return command.Execute(executionContext);
        }

        public object Execute(List<List<ICommand>> commands, MessageMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}
