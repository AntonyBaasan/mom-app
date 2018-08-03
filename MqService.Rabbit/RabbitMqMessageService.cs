using System;
using System.Collections.Generic;
using System.Text;
using MqService.Attributes;
using MqService.Messages;
using Newtonsoft.Json;
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
            factory = new ConnectionFactory() { HostName = connection, Port = port, DispatchConsumersAsync = true };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            broadcastMessageProcessor = new BroadcastMessageProcessor();
            directMessageProcessor = new DirectMessageProcessor();
        }

        public void Publish(string channel, IMessage message)
        {
            Publish(channel, message, "");
        }

        public void Publish(IMessage message)
        {
            Publish("", message, "");
        }

        public void Publish(IMessage message, string route)
        {
            Publish("", message, route);
        }

        private void Publish(string channel, IMessage message, string route)
        {
            MessageAttribute messageAttribute = GetMessageAttribute(message.GetType());
            ValidateAttribute(messageAttribute, route);

            string queueName = string.IsNullOrEmpty(channel) ? message.GetType().FullName : channel;

            if (messageAttribute.IsBroadcast)
            {
                broadcastMessageProcessor.Publish(_channel, queueName, messageAttribute.Durable, message, route);
            }
            else
            {
                directMessageProcessor.Publish(_channel, queueName, messageAttribute.Durable, message);
            }
        }

        public KeyValuePair<string, object> ListenMessage<T>(string channel, Action<T> callback) where T : IMessage
        {
            return ListenMessage(channel, callback, new string[] { });
        }

        public KeyValuePair<string, object> ListenMessage<T>(Action<T> callback) where T : IMessage
        {
            return ListenMessage("", callback, new string[] { });
        }

        public KeyValuePair<string, object> ListenMessage<T>(Action<T> callback, string[] routes) where T : IMessage
        {
            return ListenMessage("", callback, routes);
        }

        private KeyValuePair<string, object> ListenMessage<T>(string channel, Action<T> callback, string[] routes) where T : IMessage
        {
            MessageAttribute messageAttribute = GetMessageAttribute(typeof(T));
            ValidateAttribute(messageAttribute, routes);

            string queueName = string.IsNullOrEmpty(channel) ? typeof(T).FullName : channel;

            if (messageAttribute.IsBroadcast)
            {
                var broadcastAttribute = (BroadcastMessageAttribute)messageAttribute;
                return broadcastMessageProcessor.ListenRabbitMessage(_channel, queueName, messageAttribute.Durable, callback, routes, broadcastAttribute.Target);
            }
            else
            {
                return directMessageProcessor.ListenRabbitMessage(_channel, queueName, messageAttribute.Durable, callback);
            }
        }

        public void StopListen(string listenerId)
        {
            _channel.BasicCancel(listenerId);
        }

        public List<T> GetMessages<T>()
        {
            return GetMessages<T>(null);
        }

        public List<T> GetMessages<T>(string channelName)
        {
            MessageAttribute messageAttribute = GetMessageAttribute(typeof(T));
            ValidateAttribute(messageAttribute);

            string queueName = string.IsNullOrEmpty(channelName) ? typeof(T).FullName : channelName;

            if (messageAttribute.IsBroadcast)
            {
                throw new NotImplementedException();
            }
            else
            {
                uint msgCount = _channel.MessageCount(channelName);
                var result = new List<T>();
                for (int i = 0; i < msgCount; i++)
                {
                    var r = _channel.BasicGet(channelName, false);
                    if (r == null) { continue; }
                    try
                    {
                        var message = Encoding.UTF8.GetString(r.Body);
                        var msg = JsonConvert.DeserializeObject<T>(message);

                        result.Add(msg);
                        _channel.BasicAck(r.DeliveryTag, false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Can not parse a message! " + e.Message);
                        // TODO: log 
                    }
                }
                return result;
            }
        }

        private void ValidateAttribute(MessageAttribute messageAttribute, string route = null)
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
                //if (broadcaseAttr.RouteRequired == true && (route == null || route.Length == 0))
                //{
                //    throw new Exception($"Route information required for this type of message!");
                //}

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
