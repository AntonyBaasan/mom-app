using MqService.Attributes;

namespace MqService.Messages.Contents
{
    [DirectMessage]
    public class OpenModuleMessage : AbstractMessage
    {
        public string Name;
    }
}
