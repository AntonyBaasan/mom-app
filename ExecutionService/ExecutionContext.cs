using MqService;
using MqService.Messages;
using RestConsumer;

namespace ExecutionService
{
    public class ExecutionContext
    {
        public IMessageService messageService;
        public IRestClient restClient;
        public MessageMetadata metaData;
    }
}
