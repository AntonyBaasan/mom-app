using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MqService.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MqService.Rabbit
{
    class DirectMessageProcessor
    {
        public void Publish(IModel _channel, string queueName, bool durable, IMessage message)
        {
            _channel.QueueDeclare(queue: queueName, durable: durable, exclusive: false, autoDelete: false, arguments: null);

            string json = JsonConvert.SerializeObject(message);
            var jsonAsString = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: jsonAsString);
        }

        public KeyValuePair<string, object> ListenRabbitMessage<T>(IModel _channel, string channelName, bool durable, Action<T> callback) where T : IMessage
        {
            _channel.QueueDeclare(queue: channelName, durable: durable, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
             {
                 Debug.WriteLine("Received!!");
                 System.Diagnostics.EventLog.WriteEntry("Trident", "Received!!");

                 var messagePayload = Encoding.UTF8.GetString(ea.Body);
                 var msg = JsonConvert.DeserializeObject<T>(messagePayload);
                 callback(msg);
                 await Task.Yield();
             };

            return new KeyValuePair<string, object>(
                _channel.BasicConsume(queue: channelName, autoAck: true, consumer: consumer),
                consumer
                );
        }
    }
}
