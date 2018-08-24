using System;
using ExecutionService;
using MqService;
using MqService.Rabbit;
using NlpLibrary;
using RestConsumer;

namespace NlpConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var connectionString = "192.168.99.100";
            var port = 5672;

            IMessageService messageService = new RabbitMqMessageService(connectionString, port, 1, false);
            IRestClient restClient = new RestClient("http://localhost");
            IExecutionService executionService = new ExecutionService.ExecutionService(messageService, restClient);
            NlpService nlpService = new NlpService(messageService, executionService);

            Console.WriteLine("NLP service started...");
            Console.ReadKey();
        }
    }
}
