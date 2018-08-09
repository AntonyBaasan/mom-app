using System;
using System.Collections.Generic;
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
        private const string ExchangeType = "fanout";
        private const string EmptyRoute = "anonymous.info";

        public void Publish(IModel channel, string channelName, bool durable, IMessage message)
        {
            channel.ExchangeDeclareNoWait(exchange: channelName, type: ExchangeType);

            string json = JsonConvert.SerializeObject(message);
            var jsonAsString = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: channelName,
                routingKey: "",
                basicProperties: null,
                body: jsonAsString);
            Console.WriteLine(" [x] Sent fanout to '{0}'", channelName);
        }

        public string ListenRabbitMessage<T>(IModel channel, string channelName, bool durable, Action<T> callback, BroadcastTarget target) where T : IMessage
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

            channel.QueueBind(queue: queueName, exchange: channelName, routingKey: "");

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
