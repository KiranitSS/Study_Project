﻿using System;
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
        private readonly Print print;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        /// <param name="print">Records print method.</param>
        public ListCommandHandler(IFileCabinetService service, Print print)
            : base(service)
        {
            this.print = print;
        }

        public delegate void Print(IEnumerable<FileCabinetRecord> records);

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                this.List();
            }

            return base.Handle(request);
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

            this.print(records);
        }
    }
}