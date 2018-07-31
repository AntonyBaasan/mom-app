using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqService.Messages;

namespace MqService
{
    public interface IMessageService: IDisposable
    {
        void Publish(IMessage message);

        void Publish(IMessage message, string route);

        void ListenMessage<T>(Action<T> callback) where T : IMessage;

        void ListenMessage<T>(Action<T> callback, string[] routes) where T : IMessage;
    }
}
