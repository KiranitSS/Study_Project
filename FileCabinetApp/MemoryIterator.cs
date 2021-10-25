using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> records;
        private int position = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Collection of records.</param>
        public MemoryIterator(List<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            this.records = records;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            return this.records[++this.position];
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            if (this.records.Count <= this.position + 1)
            {
                return false;
            }

            return true;
        }
    }
}
