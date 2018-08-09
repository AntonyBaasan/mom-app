using System;
using MqService;
using MqService.Helper;
using MqService.Messages;
using MqService.Messages.Contents;
using MqService.Messages.Nlp;
using MqService.Rabbit;
using NlpLibrary;

namespace NlpConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var connectionString = "192.168.99.100";
            var port = 5672;

            IMessageService messageService = new RabbitMqMessageService(connectionString, port, 1, false);
            NlpService nlpService = new NlpService(messageService);

            Console.WriteLine("NLP service started...");
            Console.ReadKey();
        }
    }
}
