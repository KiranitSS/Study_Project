using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for removing record.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("remove", StringComparison.OrdinalIgnoreCase))
            {
                Remove(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Remove(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters) || !int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            Program.FileCabinetService.RemoveRecord(id);
        }
    }
}
