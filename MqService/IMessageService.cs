using System;
using MqService.Messages;

namespace MqService
{
    public interface IMessageService: IDisposable
    {
        void Publish(string channel, IMessage message);

        void Publish(IMessage message);

        void Publish(IMessage message, string route);

        void ListenMessage<T>(string channel, Action<T> callback) where T : IMessage;

        void ListenMessage<T>(Action<T> callback) where T : IMessage;

        void ListenMessage<T>(Action<T> callback, string[] routes) where T : IMessage;
    }
}
