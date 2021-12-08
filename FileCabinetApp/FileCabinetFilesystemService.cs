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
        private readonly Dictionary<string, int> baseOffSets = new Dictionary<string, int>
        {
            { "status", 0 },
            { "id", 2 },
            { "firstname", 6 },
            { "lastname", 126 },
            { "year", 246 },
            { "month", 250 },
            { "day", 254 },
            { "moneycount", 258 },
            { "pin", 274 },
            { "charprop", 276 },
        };

        private readonly SortedList<string, List<int>> fieldOffsets = new SortedList<string, List<int>>();
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

        public SortedList<string, List<int>> this[string fieldName]
        {
            get
            {
                if (!this.fieldOffsets.ContainsKey(fieldName))
                {
                    this.fieldOffsets.Add(fieldName, this.GetOffsets(fieldName));
                }
                else
                {
                    this.fieldOffsets[fieldName] = this.GetOffsets(fieldName);
                }

                return this.fieldOffsets;
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

            this.fileStream.Close();
            this.fileStream = new FileStream(this.path, FileMode.Create);

            records.ForEach(rec => this.SaveRecord(new RecordDataConverter(rec)));
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

            if (data.Id == -1)
            {
                Console.WriteLine($"Record is not created.");
            }
            else
            {
                Console.WriteLine($"Record #{data.Id} is created.");
            }

            return data.Id;
        }

        /// <inheritdoc/>
        public void UpdateRecords(Dictionary<string, string> paramsToChange, Dictionary<string, string> searchCriteria)
        {
            if (paramsToChange is null)
            {
                throw new ArgumentNullException(nameof(paramsToChange));
            }

            if (searchCriteria is null)
            {
                throw new ArgumentNullException(nameof(searchCriteria));
            }

            if (paramsToChange.Count == 0 || searchCriteria.Count == 0)
            {
                Console.WriteLine("Not enough parameters.");
                return;
            }

            var records = this.GetExistingRecords();

            List<int> matchingIndexes = ServiceUtils.FindByProp(records, searchCriteria.First().Key, searchCriteria.First().Value);

            List<int> searchingIndexes;

            foreach (var critria in searchCriteria)
            {
                searchingIndexes = ServiceUtils.FindByProp(records, critria.Key, critria.Value);
                matchingIndexes = ServiceUtils.GetMatchingIndexes(searchingIndexes, matchingIndexes);
            }

            if (matchingIndexes.Count == 0)
            {
                Console.WriteLine("No any records changed.");
                return;
            }

            int recordIndex;

            foreach (var id in matchingIndexes)
            {
                recordIndex = records.FindIndex(x => x.Id == id);
                records[recordIndex] = GetUpdatedRecord(records[recordIndex], paramsToChange);
            }

            this.fileStream.Close();
            this.fileStream = new FileStream(this.path, FileMode.Create);

            records.ForEach(rec => this.SaveRecord(new RecordDataConverter(rec)));
            this.GetRemovedRecords().ForEach(rec => this.SaveRecord(rec));
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string dateOfBirth)
        {
            bool IsSearchable(FileCabinetRecord rec)
            {
                return rec.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)
                .Replace("\0", string.Empty)
                .Equals(dateOfBirth, StringComparison.OrdinalIgnoreCase);
            }

            return new FilesystemIterator(this.path, 277, IsSearchable);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            bool IsSearchable(FileCabinetRecord rec)
            {
                return rec.FirstName
                .Replace("\0", string.Empty)
                .Equals(firstName, StringComparison.OrdinalIgnoreCase);
            }

            return new FilesystemIterator(this.path, 277, IsSearchable);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastname)
        {
            bool IsSearchable(FileCabinetRecord rec)
            {
                return rec.LastName
                .Replace("\0", string.Empty)
                .Equals(lastname, StringComparison.OrdinalIgnoreCase);
            }

            return new FilesystemIterator(this.path, 277, IsSearchable);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<RecordDataConverter> recordsData = this.GetRecordsData();

            List<FileCabinetRecord> records = recordsData.Select(rec => new FileCabinetRecord(rec)).ToList();

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.GetRecords().Count;
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
            return new FileCabinetServiceSnapshot(this.GetExistingRecords().ToArray());
        }

        /// <inheritdoc/>
        public void PurgeRecords()
        {
            var records = this.GetExistingRecords();
            this.fileStream.Close();
            this.fileStream = new FileStream(this.path, FileMode.Create);

            records.ForEach(rec => this.SaveRecord(new RecordDataConverter(rec)));
        }

        /// <inheritdoc/>
        public void InsertRecord(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var records = this.GetExistingRecords();
            int index = records.FindIndex(rec => rec.Id == parameters.Id);

            if (index < 0)
            {
                this.fileStream.Dispose();
                this.fileStream = new FileStream(this.path, FileMode.OpenOrCreate);

                this.SaveRecord(new RecordDataConverter(parameters));
            }
            else
            {
                var record = new FileCabinetRecord
                {
                    Id = parameters.Id,
                    FirstName = parameters.FirstName,
                    LastName = parameters.LastName,
                    DateOfBirth = parameters.DateOfBirth,
                    PIN = parameters.PIN,
                    MoneyCount = parameters.MoneyCount,
                    CharProp = parameters.CharProp,
                };

                records[index] = record;

                this.fileStream.Dispose();
                this.fileStream = new FileStream(this.path, FileMode.OpenOrCreate);
                records.ForEach(rec => this.SaveRecord(new RecordDataConverter(rec)));
            }
        }

        /// <inheritdoc/>
        public void DeleteRecords(string parameters)
        {
            if (parameters is null)
            {
                return;
            }

            parameters = parameters.Replace("where", string.Empty);
            int separatorIndex = parameters.IndexOf('=');

            string propName = parameters[..separatorIndex].Trim().ToUpperInvariant();
            string propValue = parameters[(separatorIndex + 1) ..].Trim().Replace("'", string.Empty).ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(propName) || string.IsNullOrWhiteSpace(propValue))
            {
                Console.WriteLine("Not enougth parameters");
                return;
            }

            var targetIndexes = ServiceUtils.FindByProp(this.GetExistingRecords(), propName, propValue);

            if (targetIndexes.Count == 0)
            {
                Console.WriteLine("No matching entries found");
                return;
            }

            if (targetIndexes.Count == 1)
            {
                this.RemoveRecord(targetIndexes[0]);
                Console.Write($"Record #{targetIndexes[0]} is deleted.\n");
            }
            else
            {
                Console.Write("Records ");

                foreach (var index in targetIndexes)
                {
                    this.RemoveRecord(index);
                }

                string[] ids = new string[targetIndexes.Count];

                for (int i = 0; i < targetIndexes.Count; i++)
                {
                    ids[i] = "#" + targetIndexes[i];
                }

                Console.Write(string.Join(", ", ids));
                Console.WriteLine(" are deleted.");
            }
        }

        /// <summary>
        /// Gets existing records.
        /// </summary>
        /// <returns>Records list.</returns>
        public List<FileCabinetRecord> GetExistingRecords()
        {
            var records = this.GetRecordsData().Where(rec => rec.Status == 0).ToList();
            return records.Select(rec => new FileCabinetRecord(rec)).ToList();
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

        private static FileCabinetRecord GetUpdatedRecord(FileCabinetRecord record, Dictionary<string, string> paramsToChange)
        {
            foreach (var parameter in paramsToChange)
            {
                switch (parameter.Key.ToUpperInvariant())
                {
                    case "ID":
                        record.Id = int.Parse(parameter.Value, CultureInfo.InvariantCulture);
                        break;
                    case "FIRSTNAME":
                        record.FirstName = parameter.Value;
                        break;
                    case "LASTNAME":
                        record.LastName = parameter.Value;
                        break;
                    case "DATEOFBIRTH":
                        record.DateOfBirth = DateTime.Parse(parameter.Value, CultureInfo.InvariantCulture);
                        break;
                    case "MONEYCOUNT":
                        record.MoneyCount = decimal.Parse(parameter.Value, CultureInfo.InvariantCulture);
                        break;
                    case "PIN":
                        record.PIN = short.Parse(parameter.Value, CultureInfo.InvariantCulture);
                        break;
                    case "CHARPROP":
                        record.CharProp = parameter.Value[0];
                        break;
                    default:
                        Console.WriteLine("Incorrect property name.");
                        break;
                }
            }

            return record;
        }

        private List<int> GetOffsets(string fieldName)
        {
            int fieldOffset = this.baseOffSets[fieldName];
            int recordsCount = this.GetStat();
            List<int> offsets = new List<int>();

            for (int i = 1; i < recordsCount + 1; i++)
            {
                offsets.Add(fieldOffset * i);
            }

            return offsets;
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
                return this.GetExistingRecords().Max(rec => rec.Id);
            }
        }

        private void SaveRecord(RecordDataConverter data)
        {
            if (!this.fileStream.CanWrite)
            {
                this.fileStream.Close();
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

        private List<RecordDataConverter> GetRemovedRecords()
        {
            return this.GetRecordsData().Where(rec => rec.Status == 1).ToList();
        }

        private List<RecordDataConverter> GetRecordsData()
        {
            int size = 277;
            int recordIndex = 0;

            List<RecordDataConverter> records = new List<RecordDataConverter>();
            this.fileStream.Close();
            using (FileStream fs = new FileStream(this.path, FileMode.OpenOrCreate))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        records.Add(RecordsReader.GetRecord(size, recordIndex, fs, reader));
                        recordIndex++;
                    }
                }
            }

            return records;
        }

        private void RemoveRecord(int id)
        {
            var records = this.GetExistingRecords();
            var removedRecords = this.GetRemovedRecords();

            if (records is null)
            {
                Console.WriteLine($"Record #{id} doesn't exists.");
                return;
            }

            try
            {
                var recordForRemoving = records.Find(rec => rec.Id == id);

                if (recordForRemoving is null)
                {
                    Console.WriteLine($"Record #{id} doesn't exists.");
                    return;
                }

                var recordData = new RecordDataConverter(recordForRemoving);

                records.Remove(recordForRemoving);
                recordData.Status = 1;

                removedRecords.Add(recordData);

                this.fileStream.Close();
                this.fileStream = new FileStream(this.path, FileMode.Create);

                records.ForEach(rec => this.SaveRecord(new RecordDataConverter(rec)));
                removedRecords.ForEach(rec => this.SaveRecord(rec));
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{id} doesn't exists.");
            }
        }
    }
}
