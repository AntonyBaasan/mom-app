using System;
using MqService;
using MqService.Helper;
using MqService.Messages;
using MqService.Messages.Contents;
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

            Console.WriteLine("NLP service");
            IMessageService messageService = new RabbitMqMessageService(connectionString, port);
            NlpService nlpService = new NlpService(messageService);

            while (true)
            {
                try
                {
                    Console.WriteLine("Insert user name:");
                    var userId = Console.ReadLine();
                    Console.WriteLine("Insert text:");
                    var text = Console.ReadLine();
                    //nlpService.SendText(inputString);
                    //Console.WriteLine("Sent a message: " + inputString);

                    messageService.Publish(
                        QueueNameResolver.GetUserQueueName(userId),
                        new UserQueueMessage
                        {
                            UserId = userId,
                            contentType = MessageContentType.NotificationText,
                            Content = new NotificationText { Text = text }
                        });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
