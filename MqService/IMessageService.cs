using System;
using System.Collections.Generic;
using MqService.Helper;
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

        void Send(Channels channelName, ChannelType channelType, IMessage message);

        string Listen(Channels channelName, ChannelType channelType, Action<IMessage> callback);

        void StopListen(string listenerId);

        List<IMessage> GetMessages(string channelName, ChannelType channelType);

        //object CallRPC(IMessage message);

        //void  AcceptRPC(Func<IMessage, object> callback);
    }

}
