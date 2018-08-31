using System;
using System.Text;
using MqService.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MqService.Rabbit
{
    class BroadcastMessageProcessor
    {
        private const string ExchangeType = "fanout";
        private const string EmptyRoute = "anonymous.info";

        public void Publish(IModel channel, string channelName, bool durable, IMessage message)
        {
            channel.ExchangeDeclareNoWait(exchange: channelName.ToString(), type: ExchangeType);

            string json = JsonConvert.SerializeObject(message);
            var jsonAsString = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: channelName.ToString(),
                routingKey: "",
                basicProperties: null,
                body: jsonAsString);
            Console.WriteLine(" [x] Sent fanout to '{0}'", channelName);
        }

        public string ListenRabbitMessage<T>(IModel channel, string channelName, Action<T> callback) where T : IMessage
        {
            channel.ExchangeDeclare(exchange: channelName.ToString(), type: ExchangeType);

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, exchange: channelName.ToString(), routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                var routingKey = ea.RoutingKey;
                var msg = JsonConvert.DeserializeObject<T>(message);
                callback(msg);
            };

            return channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private string GetApplicationName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }
    }
}
