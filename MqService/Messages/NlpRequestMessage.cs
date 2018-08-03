using MqService.Attributes;

namespace MqService.Messages
{
    [DirectMessage]
    public class NlpRequestMessage : AbstractMessage
    {
        public string Text;
        public byte[] Audio;
    }
}
