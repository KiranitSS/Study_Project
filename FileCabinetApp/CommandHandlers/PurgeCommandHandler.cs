using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for clearing deleted records.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("purge", StringComparison.OrdinalIgnoreCase))
            {
                Purge();
            }

            return base.Handle(request);
        }

        private static void Purge()
        {
            Program.FileCabinetService.PurgeRecords();
        }
    }
}
