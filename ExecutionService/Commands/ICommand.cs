using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionService.Commands
{
    public interface ICommand
    {
        bool isThisCommand(string commandName);

        object Execute();
    }
}
