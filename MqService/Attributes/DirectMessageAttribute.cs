using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqService.Attributes
{
    public class DirectMessageAttribute : MessageAttribute
    {
        public override bool IsBroadcast { get => false; }
    }
}
