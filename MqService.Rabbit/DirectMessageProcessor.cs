using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MqService.Helper;
using MqService.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MqService.Rabbit
{
    class DirectMessageProcessor
    {
        public void Publish(IModel channel, Channels channelName, bool durable, IMessage message, string expiration)
        {
            //important to use no wait, otherwise throws an exception if we try to publish inside Consumer handler
            channel.QueueDeclareNoWait(queue: channelName.ToString(), durable: durable, exclusive: false, autoDelete: false, arguments: null);

            string json = JsonConvert.SerializeObject(message);
            var jsonAsString = Encoding.UTF8.GetBytes(json);

            IBasicProperties props = channel.CreateBasicProperties();
            if (!string.IsNullOrEmpty(expiration))
            {
                props.Expiration = expiration;
            }
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("AssemblyQualifiedName", message.GetType().AssemblyQualifiedName);

            channel.BasicPublish(exchange: "",
                routingKey: channelName.ToString(),
                basicProperties: props,
                body: jsonAsString);
        }

        public string ListenRabbitMessage(IModel channel, Channels channelName,  Action<IMessage> callback) 
        {
            channel.QueueDeclare(queue: channelName.ToString(), durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
             {
                 try
                 {
                     Debug.WriteLine("Received!!");

                     var assemblyQualifiedName = Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["AssemblyQualifiedName"]);
                     var messagePayload = Encoding.UTF8.GetString(ea.Body);
                     var msg = JsonConvert.DeserializeObject(messagePayload, Type.GetType(assemblyQualifiedName));
                     callback(msg as IMessage);
                     await Task.Yield();
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine(ex.Message);
                 }
             };

            return channel.BasicConsume(queue: channelName.ToString(), autoAck: true, consumer: consumer);
        }
    }
}
