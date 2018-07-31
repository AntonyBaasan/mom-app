using System;
using ExecutionServiceLibrary;
using MqService;
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
            IMessageService messageService = new RabbitMqMessageService(connectionString, port);
            ExecutionService engine = new ExecutionService(messageService);

            Console.WriteLine("Which user to listen:");
            var userId = Console.ReadLine();
            messageService.ListenMessage<UserNotificationMessage>((msg) =>
            {
                Console.WriteLine($"Got a message from {msg.UserId}, text: {msg.Text}");
            }, new string[] { userId });
            Console.ReadKey();

        }
    }
}
