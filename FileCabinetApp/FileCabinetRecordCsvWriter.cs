using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

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
                record.MoneyCount,
                record.PIN,
                record.CharProp);

            this.writer.WriteLine(formattedRecord);
        }
    }
}
