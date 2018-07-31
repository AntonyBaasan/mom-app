using System;
using System.Text;
using MqService.Attributes;
using MqService.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MqService.Rabbit
{
    class BroadcastMessageProcessor
    {
        private const string ExchangeType = "topic";
        private const string EmptyRoute = "anonymous.info";

        public void Publish(IModel channel, string queueName, bool durable, IMessage message, string route = "")
        {
            channel.ExchangeDeclare(exchange: queueName, type: ExchangeType);

            var routingKey = (route.Length > 0) ? route : EmptyRoute;

            string json = JsonConvert.SerializeObject(message);
            var jsonAsString = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: queueName,
                routingKey: routingKey,
                basicProperties: null,
                body: jsonAsString);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        }

        public void ListenRabbitMessage<T>(IModel channel, string channelName, bool durable, Action<T> callback, string[] routes, BroadcastTarget target) where T : IMessage
        {
            channel.ExchangeDeclare(exchange: channelName, type: ExchangeType);

            var queueName = "";
            if (target == BroadcastTarget.All)
            {
                queueName = channel.QueueDeclare().QueueName;
            }
            else
            {
                queueName = channelName + "_" + GetApplicationName();
                channel.QueueDeclare(queue: queueName, durable: durable, exclusive: false, autoDelete: false, arguments: null);
            }

            if (routes.Length < 1)
            {
                routes = new string[] { EmptyRoute };
            }

            foreach (var bindingKey in routes)
            {
                channel.QueueBind(queue: queueName, exchange: channelName, routingKey: bindingKey);
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                var routingKey = ea.RoutingKey;
                var msg = JsonConvert.DeserializeObject<T>(message);
                callback(msg);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private string GetApplicationName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }
    }
}
