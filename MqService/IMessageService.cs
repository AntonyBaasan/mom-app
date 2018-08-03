using System;
using System.Collections.Generic;
using MqService.Messages;

namespace MqService
{
    public interface IMessageService: IDisposable
    {
        void Publish(string channelName, IMessage message);

        void Publish(IMessage message);

        void Publish(IMessage message, string route);

        string ListenMessage<T>(string channelName, Action<T> callback) where T : IMessage;

        string ListenMessage<T>(Action<T> callback) where T : IMessage;

        string ListenMessage<T>(Action<T> callback, string[] routes) where T : IMessage;

        void StopListen(string listenerId);

        List<T> GetMessages<T>();

        List<T> GetMessages<T>(string channelName);
    }
}
