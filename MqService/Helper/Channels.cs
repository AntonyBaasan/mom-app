namespace MqService.Helper
{
    public sealed class Channels
    {
        private readonly string name;
        private readonly int value;

        private static readonly string executionChannelName = "EXECUTION";
        private static readonly string nlpChannelName = "NLP";
        private static readonly string tridentChannelName = "TRIDENT_USER";

        public static readonly Channels EXECUTION = new Channels(1, executionChannelName);
        public static readonly Channels NLP = new Channels(2, nlpChannelName);
        public static readonly Channels TRIDENT_USER = new Channels(3, tridentChannelName);

        private Channels(int value, string name)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
