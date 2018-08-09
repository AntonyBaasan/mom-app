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
        private readonly bool _rpcAllowed;
        private ConnectionFactory factory;
        private IConnection _connection;
        private IModel _channel;
        private BroadcastMessageProcessor broadcastMessageProcessor;
        private DirectMessageProcessor directMessageProcessor;
        private RpcMessageProcessor rpcMessageProcessor;


        public RabbitMqMessageService(string connection, int port, int autoRecoveryIntervalMinutes = 1, bool rpcAllowed = true)
        {
            _rpcAllowed = rpcAllowed;
            factory = new ConnectionFactory() { HostName = connection, Port = port, DispatchConsumersAsync = true };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromMinutes(autoRecoveryIntervalMinutes);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            broadcastMessageProcessor = new BroadcastMessageProcessor();
            directMessageProcessor = new DirectMessageProcessor();
            if (_rpcAllowed) rpcMessageProcessor = new RpcMessageProcessor(_channel);
        }

        public void Publish(IMessage message)
        {
            Publish("", message);
        }

        public bool IsConnected()
        {
            return _connection.IsOpen;
        }

        public void Publish(string channelName, IMessage message)
        {
            var messageAttribute = GetCustomAttribute<MessageAttribute>(message.GetType());
            ValidateAttribute(messageAttribute);

            string queueName = string.IsNullOrEmpty(channelName) ? message.GetType().FullName : channelName;

            if (messageAttribute.IsBroadcast)
            {
                broadcastMessageProcessor.Publish(_channel, queueName, messageAttribute.Durable, message);
            }
            else
            {
                var directMessageAttribute = GetCustomAttribute<DirectMessageAttribute>(message.GetType());
                var expiry = !string.IsNullOrEmpty(message.GetExpiration()) ? message.GetExpiration() : directMessageAttribute.Expiration;
                directMessageProcessor.Publish(_channel, queueName, messageAttribute.Durable, message, expiry);
            }
        }

        public string ListenMessage<T>(Action<T> callback) where T : IMessage
        {
            return ListenMessage("", callback);
        }

        public string ListenMessage<T>(string channel, Action<T> callback) where T : IMessage
        {
            var messageAttribute = GetCustomAttribute<MessageAttribute>(typeof(T));
            ValidateAttribute(messageAttribute);

            string queueName = string.IsNullOrEmpty(channel) ? typeof(T).FullName : channel;

            if (messageAttribute.IsBroadcast)
            {
                var broadcastAttribute = (BroadcastMessageAttribute)messageAttribute;
                return broadcastMessageProcessor.ListenRabbitMessage(_channel, queueName, messageAttribute.Durable, callback, broadcastAttribute.Target);
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
            var messageAttribute = GetCustomAttribute<MessageAttribute>(typeof(T));
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

        public object CallRPC<T>(T message) where T : IMessage
        {
            if (_rpcAllowed)
            {
                throw new NotSupportedException("RPC calls not supported!");
            }
            return rpcMessageProcessor.CallRPC(message);
        }

        public void AcceptRPC<T>(Func<T, object> callback) where T : IMessage
        {
            if (_rpcAllowed)
            {
                throw new NotSupportedException("RPC calls not supported!");
            }
            rpcMessageProcessor.AcceptRPC(callback);
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

        private T GetCustomAttribute<T>(Type messageType) where T : Attribute
        {
            foreach (Attribute attribute in messageType.GetCustomAttributes(false))
            {
                if (attribute is T)
                {
                    return (T)attribute;
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
