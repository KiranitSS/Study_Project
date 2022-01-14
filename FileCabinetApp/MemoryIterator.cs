using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents iterator for traversing records from collection.
    /// </summary>
    public class MemoryIterator : IEnumerator<FileCabinetRecord>, IEnumerable<FileCabinetRecord>
    {
        private readonly List<FileCabinetRecord> records;
        private int position = -1;
        private bool disposedValue;

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
        public FileCabinetRecord Current => this.records[this.position];

        /// <inheritdoc/>
        object IEnumerator.Current => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool MoveNext()
        {
            return ++this.position < this.records.Count;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.position = -1;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            for (int i = 0; i < this.records.Count; i++)
            {
                yield return this.records[i];
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.records).GetEnumerator();
        }

        /// <summary>
        /// Dispose iterator.
        /// </summary>
        /// <param name="disposing">Is disposing launched.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                this.disposedValue = true;
            }
        }
    }
}
