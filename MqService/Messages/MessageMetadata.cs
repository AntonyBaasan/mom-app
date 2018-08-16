using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqService.Messages
{
    public class MessageMetadata
    {
        public string Expiration { get; set; }
        public UserInfo RequestUserInfo { get; set; }
    }
}
