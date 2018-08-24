using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqService.Messages;

namespace ExecutionService.Commands
{
    public interface ICommand
    {
        object Execute(ExecutionContext context);
        void SetSender(UserInfo userInfo);
    }
}
