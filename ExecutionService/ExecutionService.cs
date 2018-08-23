using System;
using System.Collections.Generic;
using ExecutionService.Commands;
using MqService.Messages;

namespace ExecutionService
{
    public class ExecutionService : IExecutionService
    {
        public object Execute(ICommand command, UserInfo sender)
        {
            return command.Execute();
        }

        public object Execute(List<List<ICommand>> commands, UserInfo sender)
        {
            throw new NotImplementedException();
        }
    }
}
