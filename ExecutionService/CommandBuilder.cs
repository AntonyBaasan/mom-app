using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecutionService.Commands;

namespace ExecutionService
{
    public class CommandBuilder
    {
        /// <summary>
        /// if command is in string format has to have very specific grammer JSON.
        /// {
        ///     commandType: "OpenModule",
        ///     moduleName: "AdHoc"
        /// }
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ICommand Create(string command)
        {
            return null;
        }
    }
}
