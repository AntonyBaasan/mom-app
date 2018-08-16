using System;
using System.Collections.Generic;
using System.Text;
using MqService.Attributes;
using MqService.Helper;
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
        private IModel _model;
        private BroadcastMessageProcessor _broadcastMessageProcessor;
        private DirectMessageProcessor _directMessageProcessor;
        //private RpcMessageProcessor _rpcMessageProcessor;


        public RabbitMqMessageService(string connection, int port, int autoRecoveryIntervalMinutes = 1, bool rpcAllowed = true)
        {
            _rpcAllowed = rpcAllowed;
            factory = new ConnectionFactory() { HostName = connection, Port = port, DispatchConsumersAsync = true };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromMinutes(autoRecoveryIntervalMinutes);

            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();

            _broadcastMessageProcessor = new BroadcastMessageProcessor();
            _directMessageProcessor = new DirectMessageProcessor();
            //if (_rpcAllowed) _rpcMessageProcessor = new RpcMessageProcessor(_model);
        }

        public bool IsConnected()
        {
            return _connection.IsOpen;
        }

        public void Send(Channels channelName, ChannelType channelType, IMessage message)
        {
            var messageAttribute = GetCustomAttribute<MessageAttribute>(message.GetType());
            ValidateAttribute(messageAttribute);

            if (channelType == ChannelType.Broadcast)
            {
                _broadcastMessageProcessor.Publish(_model, channelName, messageAttribute.Durable, message);
            }
            else
            {
                var directMessageAttribute = GetCustomAttribute<DirectMessageAttribute>(message.GetType());
                var expiry = !string.IsNullOrEmpty(message.Metadata.Expiration) ? message.Metadata.Expiration : directMessageAttribute.Expiration;
                _directMessageProcessor.Publish(_model, channelName, messageAttribute.Durable, message, expiry);
            }
        }

        public string Listen(Channels channelName, ChannelType channelType, Action<IMessage> callback)
        {
            if (channelType == ChannelType.Broadcast)
            {
                return _broadcastMessageProcessor.ListenRabbitMessage(_model, channelName, callback);
            }
            else
            {
                return _directMessageProcessor.ListenRabbitMessage(_model, channelName, callback);
            }
        }

        public void StopListen(string listenerId)
        {
            _model.BasicCancel(listenerId);
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
                uint msgCount = _model.MessageCount(channelName);
                var result = new List<IMessage>();
                for (int i = 0; i < msgCount; i++)
                {
                    var r = _model.BasicGet(channelName, false);
                    if (r == null) { continue; }
                    try
                    {
                        var message = Encoding.UTF8.GetString(r.Body);
                        var msg = JsonConvert.DeserializeObject<IMessage>(message);

                        result.Add(msg);
                        _model.BasicAck(r.DeliveryTag, false);
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
            throw new NotImplementedException();
            //if (_rpcAllowed)
            //{
            //    throw new NotSupportedException("RPC calls not supported!");
            //}
            //return _rpcMessageProcessor.CallRPC(message);
        }

        public void AcceptRPC(Func<IMessage, object> callback)
        {
            throw new NotImplementedException();
            //if (_rpcAllowed)
            //{
            //    throw new NotSupportedException("RPC calls not supported!");
            //}
            //_rpcMessageProcessor.AcceptRPC(callback);
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
            _model?.Dispose();
            _model = null;

            _connection?.Dispose();
            _connection = null;
        }

    }
}
