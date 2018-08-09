using MqService.Attributes;

namespace MqService.Messages.Nlp
{
    [DirectMessage]
    public class NlpResponseMessage : AbstractMessage
    {
        public string Response;
        public string To;
    }
}
