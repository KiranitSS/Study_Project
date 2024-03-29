﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for importing records.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("import", StringComparison.OrdinalIgnoreCase))
            {
                this.Import(request.Parameters);
            }

            return base.Handle(request);
        }

        private static bool IsAbleToImport(string[] importParams)
        {
            if (importParams.Length != 2)
            {
                Console.WriteLine("Incorrect input parameters");
                return false;
            }

            if (!File.Exists(importParams[1]))
            {
                Console.WriteLine($"Import error: file {importParams[1]} is not exist.");
                return false;
            }

            return true;
        }

        private void Import(string parameters)
        {
            parameters = parameters.Trim();

            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Not enougth parameters");
                return;
            }

            string[] importParams = parameters.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (!IsAbleToImport(importParams))
            {
                return;
            }

            using (StreamReader reader = new StreamReader(importParams[1]))
            {
                if (importParams[0].Equals("csv"))
                {
                    FileCabinetServiceSnapshot snapshot = this.Service.MakeSnapshot();
                    snapshot.LoadFromCsv(reader, Program.Validator);
                    this.Service.Restore(snapshot);
                }

                if (importParams[0].Equals("xml"))
                {
                    FileCabinetServiceSnapshot snapshot = this.Service.MakeSnapshot();
                    snapshot.LoadFromXml(reader, Program.Validator);
                    this.Service.Restore(snapshot);
                }
            }
        }
    }
}
