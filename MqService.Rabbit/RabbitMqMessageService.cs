using System;
using MqService.Attributes;
using MqService.Messages;
using RabbitMQ.Client;

namespace MqService.Rabbit
{
    public class RabbitMqMessageService : IMessageService
    {
        private ConnectionFactory factory;
        private IConnection _connection;
        private IModel _channel;
        private BroadcastMessageProcessor broadcastMessageProcessor;
        private DirectMessageProcessor directMessageProcessor;

        public RabbitMqMessageService(string connection, int port)
        {
            factory = new ConnectionFactory() { HostName = connection, Port = port };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            broadcastMessageProcessor = new BroadcastMessageProcessor();
            directMessageProcessor = new DirectMessageProcessor();
        }

        public void Publish(IMessage message)
        {
            Publish(message, "");
        }

        public void Publish(IMessage message, string route)
        {
            MessageAttribute messageAttribute = GetMessageAttribute(message.GetType());
            ValidateAttribute(messageAttribute, route);


            if (messageAttribute.IsBroadcast)
            {
                broadcastMessageProcessor.Publish(_channel, message.GetType().FullName, messageAttribute.Durable, message, route);
            }
            else
            {
                directMessageProcessor.Publish(_channel, message.GetType().FullName, messageAttribute.Durable, message);
            }
        }

        public void ListenMessage<T>(Action<T> callback) where T : IMessage
        {
            ListenMessage(callback, new string[] { });
        }

        public void ListenMessage<T>(Action<T> callback, string[] routes) where T : IMessage
        {
            MessageAttribute messageAttribute = GetMessageAttribute(typeof(T));
            ValidateAttribute(messageAttribute, routes);

            if (messageAttribute.IsBroadcast)
            {
                var broadcastAttribute = (BroadcastMessageAttribute)messageAttribute;
                broadcastMessageProcessor.ListenRabbitMessage(_channel, typeof(T).FullName, messageAttribute.Durable, callback, routes, broadcastAttribute.Target);
            }
            else
            {
                directMessageProcessor.ListenRabbitMessage(_channel, typeof(T).FullName, messageAttribute.Durable, callback);
            }
        }

        private void ValidateAttribute(MessageAttribute messageAttribute, string route)
        {
            string[] routes = string.IsNullOrEmpty(route) ? null : new string[] { route };
            ValidateAttribute(messageAttribute, routes);
        }

        private void ValidateAttribute(MessageAttribute messageAttribute, string[] route)
        {
            if (messageAttribute == null)
            {
                throw new Exception("MessageAttribute is missing!");
            }
            if (messageAttribute.IsBroadcast)
            {
                var broadcaseAttr = (BroadcastMessageAttribute)messageAttribute;
                if (broadcaseAttr.RouteRequired == true && (route == null || route.Length == 0))
                {
                    throw new Exception($"Route information required for this type of message!");
                }

                if (broadcaseAttr.Target == BroadcastTarget.Application && !(route == null || route.Length == 0))
                {
                    throw new Exception($"Usage of 'Route' and 'BroadcastTarget.Application' will introduce not logical result!");
                }
            }
        }

        private MessageAttribute GetMessageAttribute(Type messageType)
        {
            foreach (Attribute attribute in messageType.GetCustomAttributes(false))
            {
                if (attribute is MessageAttribute)
                {
                    return (MessageAttribute)attribute;
                }
            }
            return null;
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

    }
}
