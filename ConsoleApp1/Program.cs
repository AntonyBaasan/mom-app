using System;
using MqService;
using MqService.Helper;
using MqService.Messages;
using MqService.Messages.Contents;
using MqService.Rabbit;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var connectionString = "192.168.99.100";
            var port = 5672;

            Console.WriteLine("NLP service");
            using (IMessageService messageService = new RabbitMqMessageService(connectionString, port))
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine("User name:");
                        var userId = Console.ReadLine();
                        Console.WriteLine("Text:");
                        var text = Console.ReadLine();

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
}
