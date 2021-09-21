using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents class to save records data in csv file.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer with established path.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes records information to a csv file.
        /// </summary>
        /// <param name="record">Record whose information is written.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var formattedRecord = new StringBuilder();
            formattedRecord.AppendJoin(
                ',',
                record.Id,
                record.FirstName,
                record.LastName,
                record.DateOfBirth,
                record.MoneyCount.ToString(CultureInfo.InvariantCulture),
                record.PIN,
                record.CharProp);

            this.writer.WriteLine(formattedRecord);
        }
    }
}
