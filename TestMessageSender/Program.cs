using System;
using MqService;
using MqService.Helper;
using MqService.Messages;
using MqService.Messages.Nlp;
using MqService.Rabbit;

namespace TestMessageSender
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var connectionString = "192.168.99.100";
            var port = 5672;

            Console.WriteLine("NLP service");
            using (IMessageService messageService = new RabbitMqMessageService(connectionString, port, 1, false))
            {
                //messageService.Listen(Channels.TRIDENT_USER, ChannelType.Direct, message =>
                //{
                //    Console.WriteLine("Received msg : " + message.ToString());
                //});
                //messageService.ListenMessage<UserQueueMessage>(/*QueueNameResolver*/.GetUserQueueName("1"), msg =>
                //{
                //    Console.WriteLine("Received UserQueueMessage 1 " + msg.ContentTypeFullName);
                //});
                //messageService.ListenMessage<NlpResponseMessage>(msg =>
                //{
                //    Console.WriteLine("Received NlpResponseMessage: " + msg.Response);
                //});

                while (true)
                {
                    try
                    {
                        Publish_NlpRequest(messageService);
                        //Publish_NotificationMessage(messageService);
                        //CallRpc_NlpRequestMessage(messageService);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private static void Publish_NlpRequest(IMessageService messageService)
        {
            Console.WriteLine("From user:");
            var userId = Console.ReadLine();
            Console.WriteLine("Text:");
            var text = Console.ReadLine();

            var msg = new NlpRequestMessage
            {
                Metadata = new MessageMetadata()
                {
                    RequestUserInfo = new UserInfo() { UserId = userId }, 
                    Expiration = "10000"
                },
                Text = text,
            };

            messageService.Send(Channels.NLP, ChannelType.Direct, msg);
        }

        private static void Publish_NotificationMessage(IMessageService messageService)
        {
            Console.WriteLine("User name:");
            var userId = Console.ReadLine();
            Console.WriteLine("Text:");
            var text = Console.ReadLine();

            var msg = new NotificationMessage
            {
                To = new String[] { userId },
                Text = text,
                Metadata = new MessageMetadata() { RequestUserInfo = new UserInfo() { UserId = "Tester" }, Expiration = "10000" },
            };

            messageService.Send(Channels.TRIDENT_USER, ChannelType.Direct, msg);
        }

        private static void CallRpc_NlpRequestMessage(IMessageService messageService)
        {
            Console.WriteLine("Text:");
            var text = Console.ReadLine();

            //var result1 = messageService.CallRPC(new NlpRequestMessage() { Text = text });
            //Console.WriteLine("result1: " + result1.ToString());
        }
    }
}
