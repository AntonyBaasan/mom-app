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

        public bool IsConnected()
        {
            return _connection.IsOpen;
        }

        public void Publish(string channelName, ChannelType channelType, IMessage message)
        {
            var messageAttribute = GetCustomAttribute<MessageAttribute>(message.GetType());
            ValidateAttribute(messageAttribute);

            if (channelType == ChannelType.Broadcast)
            {
                broadcastMessageProcessor.Publish(_channel, channelName, messageAttribute.Durable, message);
            }
            else
            {
                var directMessageAttribute = GetCustomAttribute<DirectMessageAttribute>(message.GetType());
                var expiry = !string.IsNullOrEmpty(message.GetExpiration()) ? message.GetExpiration() : directMessageAttribute.Expiration;
                directMessageProcessor.Publish(_channel, channelName, messageAttribute.Durable, message, expiry);
            }
        }

        public string Listen(string channel, ChannelType channelType, Action<IMessage> callback, bool durable = false)
        {
            if (channelType == ChannelType.Broadcast)
            {
                return broadcastMessageProcessor.ListenRabbitMessage(_channel, channel, callback);
            }
            else
            {
                return directMessageProcessor.ListenRabbitMessage(_channel, channel, durable, callback);
            }
        }

        public void StopListen(string listenerId)
        {
            _channel.BasicCancel(listenerId);
        }

        public List<IMessage> GetMessages(string channelName, ChannelType channelType)
        {
            //var messageAttribute = GetCustomAttribute<MessageAttribute>(typeof(T));
            //ValidateAttribute(messageAttribute);

            if (channelType == ChannelType.Broadcast)
            {
                throw new NotImplementedException();
            }
            else
            {
                uint msgCount = _channel.MessageCount(channelName);
                var result = new List<IMessage>();
                for (int i = 0; i < msgCount; i++)
                {
                    var r = _channel.BasicGet(channelName, false);
                    if (r == null) { continue; }
                    try
                    {
                        var message = Encoding.UTF8.GetString(r.Body);
                        var msg = JsonConvert.DeserializeObject<IMessage>(message);

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

        public object CallRPC(IMessage message)
        {
            if (_rpcAllowed)
            {
                throw new NotSupportedException("RPC calls not supported!");
            }
            return rpcMessageProcessor.CallRPC(message);
        }

        public void AcceptRPC(Func<IMessage, object> callback)
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
