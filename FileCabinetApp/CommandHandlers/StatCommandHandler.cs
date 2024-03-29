﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for getting count of records in storage.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("stat", StringComparison.OrdinalIgnoreCase))
            {
                this.Stat();
            }

            return base.Handle(request);
        }

        private void Stat()
        {
            int recordsCount = Program.IsLogging
                ? new ServiceLogger(new ServiceMeter(this.Service)).GetStat()
                : new ServiceMeter(this.Service).GetStat();

            if (this.Service.GetType() == typeof(FileCabinetFilesystemService))
            {
                int existingRecordsCount = (this.Service as FileCabinetFilesystemService).GetExistingRecords().Count;
                int removedRecordsCount = recordsCount - existingRecordsCount;

                Console.WriteLine($"Here {existingRecordsCount} record(s).");
                Console.WriteLine($"And here {removedRecordsCount} removed records.");

                return;
            }

            Console.WriteLine($"Here {recordsCount} record(s).\nAnd here 0 removed records.");
        }
    }
}
