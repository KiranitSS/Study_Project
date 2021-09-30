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
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public ListCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.service = service;
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

            if (this.service.GetType().Equals(typeof(FileCabinetFilesystemService)))
            {
                records = (this.service as FileCabinetFilesystemService).GetExistingRecords();
            }
            else
            {
                records = (this.service as FileCabinetMemoryService).GetRecords().ToList();
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
    }
}
