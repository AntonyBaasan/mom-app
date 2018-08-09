using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqService.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MqService.Rabbit
{
    class RpcMessageProcessor
    {
        private IModel _channel { get; }
        private readonly AsyncEventingBasicConsumer rpcConsumer;
        private readonly BlockingCollection<object> rpcRespQueue;
        private readonly string rpcReplyQueueName;

        public RpcMessageProcessor(IModel channel)
        {
            _channel = channel;

            rpcConsumer = new AsyncEventingBasicConsumer(_channel);
            rpcRespQueue = new BlockingCollection<object>();
            rpcReplyQueueName = _channel.QueueDeclare().QueueName;
        }

        public object CallRPC<T>(T message) where T : IMessage
        {
            
            var props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = rpcReplyQueueName;

            rpcConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    rpcRespQueue.Add(response);
                }
            };

            string json = JsonConvert.SerializeObject(message);
            var jsonAsString = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: jsonAsString);

            _channel.BasicConsume(
                consumer: rpcConsumer,
                queue: rpcReplyQueueName,
                autoAck: true);

            return rpcRespQueue.Take();
        }

        public void  AcceptRPC<T>(Func<T, object> callback) where T : IMessage
        {
            _channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);

            Console.WriteLine(" [x] Awaiting RPC requests");

            consumer.Received += async (model, ea) =>
            {
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var messagePayload = Encoding.UTF8.GetString(ea.Body);
                var msg = JsonConvert.DeserializeObject<T>(messagePayload);

                object response = callback(msg);
                var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps,
                    body: responseBytes);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
        }

    }
}
