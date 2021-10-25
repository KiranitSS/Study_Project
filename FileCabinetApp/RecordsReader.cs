using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Utils for records reading.
    /// </summary>
    public static class RecordsReader
    {
        /// <summary>
        /// Reads record from binary file.
        /// </summary>
        /// <param name="size">Record length in bytes.</param>
        /// <param name="recordIndex">The number of the record that is being read.</param>
        /// <param name="fs">Stream for seek setting.</param>
        /// <param name="reader">Reader for reading file.</param>
        /// <returns>Returns the data for creating new <see cref="FileCabinetRecord"/>.</returns>
        public static RecordDataConverter GetRecord(int size, int recordIndex, FileStream fs, BinaryReader reader)
        {
            if (fs is null)
            {
                throw new ArgumentNullException(nameof(fs));
            }

            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            long offset = 0;
            fs.Seek(offset, SeekOrigin.Begin);

            RecordDataConverter record = new RecordDataConverter();
            offset = size * recordIndex;
            fs.Seek(offset, SeekOrigin.Begin);

            record.Status = reader.ReadInt16();

            record.Id = reader.ReadInt32();

            byte[] firstname = reader.ReadBytes(120);
            record.SetFirstName(Encoding.UTF8.GetString(firstname).ToList());

            byte[] lastname = reader.ReadBytes(120);
            record.SetLastName(Encoding.UTF8.GetString(lastname).ToList());

            record.Year = reader.ReadInt32();
            record.Month = reader.ReadInt32();
            record.Day = reader.ReadInt32();

            record.MoneyCount = reader.ReadDecimal();
            record.PIN = reader.ReadInt16();
            record.CharProp = reader.ReadChar();
            return record;
        }
    }
}
