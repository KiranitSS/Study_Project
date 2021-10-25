using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for searching records.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Print print;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name = "service" > Service for working with records.</param>
        /// /// <param name="print">Records print method.</param>
        public FindCommandHandler(IFileCabinetService service, Print print)
            : base(service)
        {
            this.print = print;
        }

        public delegate void Print(IEnumerable<FileCabinetRecord> records);

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("find", StringComparison.OrdinalIgnoreCase))
            {
                this.Find(request.Parameters);
            }

            return base.Handle(request);
        }

        private static string GetTargetProp(string parameters)
        {
            string targetProp = parameters.Trim();

            if (string.IsNullOrEmpty(targetProp))
            {
                Console.WriteLine("Property name missed");
                return targetProp;
            }

            int endIndex = targetProp.IndexOf(" ", StringComparison.InvariantCulture);

            if (endIndex == -1)
            {
                Console.WriteLine("Property value missed");
                return targetProp;
            }

            return targetProp[..endIndex];
        }

        private static string GetTargetName(string parameters, int targetPropLength)
        {
            int startIndex = targetPropLength;
            return parameters[(startIndex + 1) ..];
        }

        private static ReadOnlyCollection<FileCabinetRecord> GetRecordsByIterator(IRecordIterator iterator)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            while (iterator.HasMore())
            {
                records.Add(iterator.GetNext());
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        private void PrintTargetRecords(ReadOnlyCollection<FileCabinetRecord> targetRecords)
        {
            if (targetRecords.Count == 0)
            {
                Console.WriteLine("There are no suitable entries");
            }
            else
            {
                this.print(targetRecords);
            }
        }

        private void Find(string parameters)
        {
            string targetProp = GetTargetProp(parameters);

            if (parameters.Length == targetProp.Length)
            {
                Console.WriteLine("Property value missed");
                return;
            }

            string targetName = GetTargetName(parameters, targetProp.Length);

            var targetRecords = this.FindTargetRecords(targetName, targetProp);

            this.PrintTargetRecords(targetRecords);
        }

        private ReadOnlyCollection<FileCabinetRecord> FindTargetRecords(string targetValue, string targetProp)
        {
            if (string.Equals(targetProp, "firstname", StringComparison.OrdinalIgnoreCase))
            {
                if (Program.IsLogging)
                {
                    return GetRecordsByIterator(new ServiceLogger(new ServiceMeter(this.Service)).FindByFirstName(targetValue));
                }

                return GetRecordsByIterator(new ServiceMeter(this.Service).FindByFirstName(targetValue));
            }

            if (string.Equals(targetProp, "lastname", StringComparison.OrdinalIgnoreCase))
            {
                if (Program.IsLogging)
                {
                    return GetRecordsByIterator(new ServiceLogger(new ServiceMeter(this.Service)).FindByLastName(targetValue));
                }

                return GetRecordsByIterator(new ServiceMeter(this.Service).FindByLastName(targetValue));
            }

            if (string.Equals(targetProp, "dateofbirth", StringComparison.OrdinalIgnoreCase))
            {
                if (Program.IsLogging)
                {
                    return GetRecordsByIterator(new ServiceLogger(new ServiceMeter(this.Service)).FindByBirthDate(targetValue));
                }

                return GetRecordsByIterator(new ServiceMeter(this.Service).FindByBirthDate(targetValue));
            }

            return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
        }
    }
}
