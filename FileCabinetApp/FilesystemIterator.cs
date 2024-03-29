﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents iterator for traversing records from db file.
    /// </summary>
    public class FilesystemIterator : IEnumerator<FileCabinetRecord>, IEnumerable<FileCabinetRecord>
    {
        private readonly string path;
        private readonly int size;
        private readonly Predicate<FileCabinetRecord> isSearchable;
        private int recordIndex;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="path">Path to records file.</param>
        /// <param name="size">Record's bit length.</param>
        /// <param name="getRecord">A func designed to get a record.</param>
        /// <param name="isSearchable">True if record is searchable.</param>
        public FilesystemIterator(string path, int size, Predicate<FileCabinetRecord> isSearchable)
        {
            this.path = path;
            this.size = size;
            this.isSearchable = isSearchable;
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current => this.GetNext();

        /// <inheritdoc/>
        object IEnumerator.Current => this.Current;

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (this.recordIndex * this.size < new FileInfo(this.path).Length)
            {
                using (FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        bool hasMore;

                        do
                        {
                            var data = RecordsReader.GetRecord(this.size, this.recordIndex, fs, reader);

                            if (data.Status == 1)
                            {
                                return false;
                            }

                            var record = new FileCabinetRecord(data);

                            if (this.isSearchable(record))
                            {
                                return true;
                            }

                            hasMore = ++this.recordIndex * this.size < new FileInfo(this.path).Length;
                        }
                        while (hasMore);
                    }
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.recordIndex = 0;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return this;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        private FileCabinetRecord GetNext()
        {
            using (FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    var record = new FileCabinetRecord(RecordsReader.GetRecord(this.size, this.recordIndex, fs, reader));
                    this.recordIndex++;
                    return record;
                }
            }
        }
    }
}
