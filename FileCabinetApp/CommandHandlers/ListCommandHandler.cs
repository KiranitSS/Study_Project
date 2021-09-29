using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class to list app.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                List();
            }

            return base.Handle(request);
        }

        private static void List()
        {
            List<FileCabinetRecord> records;

            if (Program.FileCabinetService.GetType().Equals(typeof(FileCabinetFilesystemService)))
            {
                records = (Program.FileCabinetService as FileCabinetFilesystemService).GetExistingRecords();
            }
            else
            {
                records = (Program.FileCabinetService as FileCabinetMemoryService).GetRecords().ToList();
            }

            if (records.Count == 0)
            {
                Console.WriteLine("There are no any records");
                return;
            }

            foreach (var record in records)
            {
                PrintRecord(record);
            }
        }

        private static void PrintRecord(FileCabinetRecord record)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, " +
                $"{record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{record.PIN}, {record.MoneyCount}$, {record.CharProp}");
        }
    }
}
