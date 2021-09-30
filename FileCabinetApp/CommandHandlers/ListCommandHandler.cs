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
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        /// <param name="printer">Records printer.</param>
        public ListCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                this.List();
            }

            return base.Handle(request);
        }

        private static void PrintRecord(FileCabinetRecord record)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, " +
                $"{record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{record.PIN}, {record.MoneyCount}$, {record.CharProp}");
        }

        private void List()
        {
            List<FileCabinetRecord> records;

            if (this.Service.GetType().Equals(typeof(FileCabinetFilesystemService)))
            {
                records = (this.Service as FileCabinetFilesystemService).GetExistingRecords();
            }
            else
            {
                records = (this.Service as FileCabinetMemoryService).GetRecords().ToList();
            }

            if (records.Count == 0)
            {
                Console.WriteLine("There are no any records");
                return;
            }

            this.printer.Print(records);
        }
    }
}
