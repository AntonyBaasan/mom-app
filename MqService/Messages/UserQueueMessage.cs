using System;
using MqService.Attributes;
using Newtonsoft.Json;

namespace MqService.Messages
{
    /// <summary>
    /// </summary>
    [DirectMessage]
    public class UserQueueMessage : AbstractMessage
    {
        public object Content;
        public string ContentTypeFullName;

        public object DeserializeContent()
        {
            return JsonConvert.DeserializeObject(Content.ToString(), Type.GetType(ContentTypeFullName));
        }

        public void SetContent(object content)
        {
            ContentTypeFullName = content.GetType().FullName;
            Content = content;
        }
    }
}
