using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records for snapshot.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        public void SaveToCsv(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException($"{nameof(writer)} is null.");
            }

            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
            writer.Write(typeof(FileCabinetRecord).GetProperties().ToString());

            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i]);
            }
        }
    }
}
