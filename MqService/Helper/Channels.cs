using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqService.Helper
{
    public sealed class Channels
    {
        private readonly String name;
        private readonly int value;

        private string executionChannelName = "ExecutionChannel";
        private string nlpChannelName = "NlpChannel";
        private string tridentChannelName = "TridentChannel";

        public static readonly Channels EXECUTION = new Channels (1, "EXECUTION");
        public static readonly Channels NLP = new Channels (2, "NLP");
        public static readonly Channels TRIDENT_USER = new Channels (3, "TRIDENT_USER");        

        private Channels(int value, String name){
            this.name = name;
            this.value = value;
        }

        public override String ToString(){
            return name;
        }
    }
}
