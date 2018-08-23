using System.Collections.Generic;
using ExecutionService.Commands;
using MqService.Messages;

namespace ExecutionService
{
    public interface IExecutionService
    {
        object Execute(ICommand command, UserInfo sender);
        object Execute(List<List<ICommand>> commands, UserInfo sender);
    }
}
