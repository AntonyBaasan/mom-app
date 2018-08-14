using MqService.Attributes;

namespace MqService.Messages.Nlp
{
    [DirectMessage]
    public class NlpRequestMessage : AbstractMessage
    {
        public string Text;
        public byte[] Audio;
    }
}
