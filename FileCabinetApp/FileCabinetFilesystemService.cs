﻿using System;
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
        private static int id;
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
        public int CreateRecord(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var data = new RecordDataConverter()
            {
                Status = 0,
                Id = ++id,
                Year = parameters.DateOfBirth.Year,
                Month = parameters.DateOfBirth.Month,
                Day = parameters.DateOfBirth.Day,
                MoneyCount = parameters.MoneyCount,
                PIN = parameters.PIN,
                CharProp = parameters.CharProp,
            };

            data.SetFirstName(StringFormatter.StringToChar(parameters.FirstName, 60));
            data.SetLastName(StringFormatter.StringToChar(parameters.LastName, 60));

            if (this.fileStream.CanWrite)
            {
                this.SaveRecord(data);
            }
            else
            {
                this.fileStream = new FileStream(this.path, FileMode.Append);
                this.SaveRecord(data);
            }

            return data.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameters parameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastname)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            int size = 277;
            int recordIndex = 0;
            long offset;

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            using (FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate))
            {

                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
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

                        records.Add(ConvertToRecord(record));
                        recordIndex++;
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        private void SaveRecord(RecordDataConverter data)
        {
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
                finally
                {
                    this.fileStream.Dispose();
                }
            }
        }
    }
}
