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
    public class StatCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("stat", StringComparison.OrdinalIgnoreCase))
            {
                Stat();
            }

            return base.Handle(request);
        }

        private static void Stat()
        {
            int recordsCount = Program.FileCabinetService.GetStat();

            if (Program.FileCabinetService.GetType() == typeof(FileCabinetFilesystemService))
            {
                int removedRecordsCount = recordsCount - (Program.FileCabinetService as FileCabinetFilesystemService).GetExistingRecords().Count;
                int existingRecordsCount = (Program.FileCabinetService as FileCabinetFilesystemService).GetExistingRecords().Count;

                Console.WriteLine($"Here {existingRecordsCount} record(s).");
                Console.WriteLine($"And here {removedRecordsCount} removed records.");
                return;
            }

            Console.WriteLine($"Here {recordsCount} record(s).\nAnd here 0 removed records.");
        }
    }
}
