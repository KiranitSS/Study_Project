using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FilesystemIterator : IRecordIterator
    {
        private readonly string path;
        private readonly int size;
        private readonly Predicate<FileCabinetRecord> isSearchable;
        private int recordNum;

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
        public FileCabinetRecord GetNext()
        {
            using (FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    var record = new FileCabinetRecord(RecordsReader.GetRecord(this.size, this.recordNum, fs, reader));
                    this.recordNum++;
                    return record;
                }
            }
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            if (this.recordNum * this.size < new FileInfo(this.path).Length)
            {
                using (FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        bool hasMore;

                        do
                        {
                            var data = RecordsReader.GetRecord(this.size, this.recordNum, fs, reader);

                            if (data.Status == 1)
                            {
                                return false;
                            }

                            var record = new FileCabinetRecord(data);

                            if (this.isSearchable(record))
                            {
                                return true;
                            }

                            hasMore = ++this.recordNum * this.size < new FileInfo(this.path).Length;
                        }
                        while (hasMore);
                    }
                }
            }

            return false;
        }
    }
}
