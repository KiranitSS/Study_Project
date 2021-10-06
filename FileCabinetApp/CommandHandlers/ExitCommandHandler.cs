using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class to terminate app.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Exit();
            }

            return base.Handle(request);
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }
    }
}
