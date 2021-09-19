using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a service for actions on <see cref="FileCabinetRecord"/>.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private readonly string path;
        private bool disposed;
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">Current FileStream.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;

            if (fileStream != null)
            {
                this.path = fileStream.Name;
            }
            else
            {
                this.path = string.Empty;
            }
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot serviceSnapshot)
        {
            if (serviceSnapshot is null)
            {
                throw new ArgumentNullException(nameof(serviceSnapshot));
            }

            List<FileCabinetRecord> records = serviceSnapshot.Records.ToList();

            this.fileStream = new FileStream(this.path, FileMode.Create);

            foreach (var record in records)
            {
                this.SaveRecord(new RecordDataConverter(record));
            }
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var data = new RecordDataConverter()
            {
                Status = 0,
                Id = this.ReadLastId(this.path) + 1,
                Year = parameters.DateOfBirth.Year,
                Month = parameters.DateOfBirth.Month,
                Day = parameters.DateOfBirth.Day,
                MoneyCount = parameters.MoneyCount,
                PIN = parameters.PIN,
                CharProp = parameters.CharProp,
            };

            data.SetFirstName(StringFormatter.StringToChar(parameters.FirstName, 60));
            data.SetLastName(StringFormatter.StringToChar(parameters.LastName, 60));

            this.fileStream.Close();
            this.fileStream = new FileStream(this.path, FileMode.Append);
            this.SaveRecord(data);

            return data.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            List<FileCabinetRecord> records = this.GetRecords().ToList();

            if (id > records.Count || id < 1)
            {
                Console.WriteLine("ID can't be bigger than records count or lower than zero.");
                return;
            }

            var data = new RecordDataConverter(parameters);

            records[id - 1] = new FileCabinetRecord
            {
                Id = id,
                FirstName = string.Concat(data.GetFirstName()),
                LastName = string.Concat(data.GetLastName()),
                DateOfBirth = new DateTime(data.Year, data.Month, data.Day),
                MoneyCount = data.MoneyCount,
                PIN = data.PIN,
                CharProp = data.CharProp,
            };

            this.fileStream.Dispose();
            this.fileStream = new FileStream(this.path, FileMode.Create);

            foreach (var record in records)
            {
                this.fileStream.Dispose();
                this.fileStream = new FileStream(this.path, FileMode.Append);
                this.SaveRecord(new RecordDataConverter(record));
            }

            this.fileStream.Dispose();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string dateOfBirth)
        {
            return new (this.GetRecords().Where(rec => rec.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).Equals(dateOfBirth)).ToList());
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var records = this.GetRecords();
            return new (records.Where(rec => rec.FirstName.Replace("\0", string.Empty).Equals(firstName, StringComparison.OrdinalIgnoreCase)).ToList());
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastname)
        {
            return new (this.GetRecords().Where(rec => rec.LastName.Replace("\0", string.Empty).Equals(lastname, StringComparison.OrdinalIgnoreCase)).ToList());
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            int size = 277;
            int recordIndex = 0;

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            using (FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        records.Add(GetRecord(size, ref recordIndex, fs, reader));
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            long offset = 277;
            using FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate);
            using BinaryReader reader = new BinaryReader(fs);
            return (int)(reader.BaseStream.Length / offset);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Shows, is object disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.fileStream.Dispose();
                }

                this.disposed = true;
            }
        }

        private static byte[] GetBytes(char[] value)
        {
            byte[] nameAsBytes = Encoding.UTF8.GetBytes(value);
            int nameLength = nameAsBytes.Length;

            byte[] fixedByteArray = new byte[120];

            Array.Copy(nameAsBytes, fixedByteArray, nameLength);
            return fixedByteArray;
        }

        private static FileCabinetRecord GetRecord(int size, ref int recordIndex, FileStream fs, BinaryReader reader)
        {
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

            recordIndex++;
            return ConvertToRecord(record);
        }

        private static FileCabinetRecord ConvertToRecord(RecordDataConverter data)
        {
            FileCabinetRecord record = new FileCabinetRecord
            {
                Id = data.Id,
                FirstName = string.Concat(data.GetFirstName().ToArray()),
                LastName = string.Concat(data.GetLastName().ToArray()),
                DateOfBirth = new DateTime(data.Year, data.Month, data.Day),
                MoneyCount = data.MoneyCount,
                PIN = data.PIN,
                CharProp = data.CharProp,
            };

            return record;
        }

        private int ReadLastId(string path)
        {
            this.fileStream.Close();
            this.fileStream = File.OpenRead(path);
            if (this.fileStream.Length == 0)
            {
                return 0;
            }
            else
            {
                this.fileStream.Position = this.fileStream.Length;
            }

            int lastLineNum = (int)(this.fileStream.Length / 277) - 1;

            using (BinaryReader reader = new BinaryReader(this.fileStream))
            {
                return GetRecord(277, ref lastLineNum, this.fileStream, reader).Id;
            }
        }

        private void SaveRecord(RecordDataConverter data)
        {
            if (!this.fileStream.CanWrite)
            {
                this.fileStream = new FileStream(this.path, FileMode.Append);
            }

            using (BinaryWriter writer = new BinaryWriter(this.fileStream))
            {
                try
                {
                    writer.Write(data.Status);
                    writer.Write(data.Id);

                    byte[] firstNameAsBytes = GetBytes(data.GetFirstName().ToArray());

                    writer.Write(firstNameAsBytes);

                    byte[] lastNameAsBytes = GetBytes(data.GetLastName().ToArray());

                    writer.Write(lastNameAsBytes);

                    writer.Write(data.Year);
                    writer.Write(data.Month);
                    writer.Write(data.Day);
                    writer.Write(data.MoneyCount);
                    writer.Write(data.PIN);
                    writer.Write(data.CharProp);
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("Failed to write. Reason: " + ex.Message);
                    throw;
                }
            }
        }
    }
}
