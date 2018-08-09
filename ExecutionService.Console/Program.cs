using System;
using System.Collections.Generic;
using System.Threading;
using ExecutionServiceLibrary;
using MqService;
using MqService.Helper;
using MqService.Messages;
using MqService.Rabbit;

namespace ExecutionServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "192.168.99.100";
            var port = 5672;

            Console.WriteLine("Execution Engine");
            IMessageService messageService = new RabbitMqMessageService(connectionString, port, 1, false);
            ExecutionService engine = new ExecutionService(messageService);
        }
    }
}
