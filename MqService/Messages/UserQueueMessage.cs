using System;
using MqService.Attributes;
using MqService.Messages.Contents;
using Newtonsoft.Json;

namespace MqService.Messages
{
    /// <summary>
    /// </summary>
    [DirectMessage]
    public class UserQueueMessage: IMessage
    {
        public string UserId;
        public MessageContentType contentType;
        public object Content;

        public object DeserializeContent()
        {
            if(contentType == MessageContentType.NotificationText)
                return JsonConvert.DeserializeObject<NotificationText>(Content.ToString());

            return null;
        }
    }
}
