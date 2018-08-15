using System;
using System.Collections.Generic;
using MqService.Messages;

namespace MqService
{
    
    public enum ChannelType
    {
        Direct,
        Broadcast,
    }

    public interface IMessageService: IDisposable
    {
        bool IsConnected();

        void Publish(string channelName, ChannelType channelType, IMessage message);

        string Listen(string channel, ChannelType channelType, Action<IMessage> callback, bool durable = false);

        void StopListen(string listenerId);

        List<IMessage> GetMessages(string channelName, ChannelType channelType);

        //object CallRPC(IMessage message);

        //void  AcceptRPC(Func<IMessage, object> callback);
    }

}
