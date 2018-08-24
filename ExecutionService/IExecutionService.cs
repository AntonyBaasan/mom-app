using System.Collections.Generic;
using ExecutionService.Commands;
using MqService.Messages;

namespace ExecutionService
{
    public interface IExecutionService
    {
        object Execute(ICommand command, MessageMetadata metadata);
        object Execute(List<List<ICommand>> commands, MessageMetadata metadata);
    }
}
