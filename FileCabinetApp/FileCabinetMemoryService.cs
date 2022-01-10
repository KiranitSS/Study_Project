using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a service for actions on <see cref="FileCabinetRecord"/>.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> birthdateDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly IRecordValidator validator;

        private List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Records parameters validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot serviceSnapshot)
        {
            if (serviceSnapshot is null)
            {
                throw new ArgumentNullException(nameof(serviceSnapshot));
            }

            this.records = serviceSnapshot.Records.ToList();
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                DateOfBirth = parameters.DateOfBirth,
                PIN = parameters.PIN,
                MoneyCount = parameters.MoneyCount,
                CharProp = parameters.CharProp,
            };

            this.AddRecordToFilterDictionaries(record);

            this.records.Add(record);

            if (record.Id == -1)
            {
                Console.WriteLine($"Record is not created.");
            }
            else
            {
                Console.WriteLine($"Record #{record.Id} is created.");
            }

            return record.Id;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var recordsCopy = new FileCabinetRecord[this.records.Count];

            for (int i = 0; i < this.records.Count; i++)
            {
                recordsCopy[i] = new FileCabinetRecord
                {
                    FirstName = this.records[i].FirstName,
                    LastName = this.records[i].LastName,
                    DateOfBirth = this.records[i].DateOfBirth,
                    PIN = this.records[i].PIN,
                    MoneyCount = this.records[i].MoneyCount,
                    CharProp = this.records[i].CharProp,
                    Id = this.records[i].Id,
                };
            }

            return new ReadOnlyCollection<FileCabinetRecord>(recordsCopy);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.records.Count;
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

            List<int> matchingIndexes = ServiceUtils.FindByProp(this.records, searchCriteria.First().Key, searchCriteria.First().Value);

            List<int> searchingIndexes;

            foreach (var critria in searchCriteria)
            {
                searchingIndexes = ServiceUtils.FindByProp(this.records, critria.Key, critria.Value);
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
                recordIndex = this.records.FindIndex(x => x.Id == id);
                this.records[recordIndex] = GetUpdatedRecord(this.records[recordIndex], paramsToChange);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return new MemoryIterator(FindByKey(firstName, this.firstNameDictionary));
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastname)
        {
            return new MemoryIterator(FindByKey(lastname, this.lastNameDictionary));
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string dateOfBirth)
        {
            return new MemoryIterator(FindByKey(dateOfBirth, this.birthdateDictionary));
        }

        /// <summary>
        /// Creates <see cref="FileCabinetServiceSnapshot"/> instance with avaible now records.
        /// </summary>
        /// <returns>Returns <see cref="FileCabinetServiceSnapshot"/> instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.records.ToArray());
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

            var targetIndexes = ServiceUtils.FindByProp(this.records, propName, propValue);

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

        /// <inheritdoc/>
        public void PurgeRecords()
        {
            Console.WriteLine("Nothing to purge.");
        }

        /// <inheritdoc/>
        public void InsertRecord(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Id < 0)
            {
                Console.WriteLine("Incorrect ID");
            }

            int index = this.records.FindIndex(rec => rec.Id == parameters.Id);

            if (index < 0)
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

                this.AddRecordToFilterDictionaries(record);

                this.records.Add(record);
            }
            else
            {
                var currentRecord = this.records[index];
                this.firstNameDictionary.TryGetValue(currentRecord.FirstName, out List<FileCabinetRecord> currentRecords);

                currentRecords.Remove(currentRecord);

                ReplaceRecordParameters(parameters, currentRecord);

                this.AddRecordToFilterDictionaries(currentRecord);
            }
        }

        /// <inheritdoc/>
        public void SelectRecords(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Parameters can't be empty");
            }

            SelectCommandUtils.SelectRecordsData(parameters, this.records);
        }

        private static void ReplaceRecordParameters(RecordParameters parameters, FileCabinetRecord currentRecord)
        {
            currentRecord.FirstName = parameters.FirstName;
            currentRecord.LastName = parameters.LastName;
            currentRecord.DateOfBirth = parameters.DateOfBirth;
            currentRecord.PIN = parameters.PIN;
            currentRecord.MoneyCount = parameters.MoneyCount;
            currentRecord.CharProp = parameters.CharProp;
        }

        /// <summary>
        /// Creates new record parameters validator.
        /// </summary>
        /// <returns>Returns <see cref="IRecordValidator"/> object
        /// which contains record validation settings.</returns>
        private static List<FileCabinetRecord> FindByKey(string key, Dictionary<string, List<FileCabinetRecord>> filterDictionary)
        {
            bool contains = filterDictionary.TryGetValue(key, out List<FileCabinetRecord> currentRecords);

            if (!contains)
            {
                return new List<FileCabinetRecord>();
            }

            return currentRecords;
        }

        private static void AddRecordToDictionary(FileCabinetRecord record, string key, Dictionary<string, List<FileCabinetRecord>> filterDictionary)
        {
            filterDictionary.TryGetValue(key, out List<FileCabinetRecord> currentRecords);

            if (currentRecords == null)
            {
                currentRecords = new List<FileCabinetRecord>();
            }

            currentRecords.Add(record);

            if (!filterDictionary.ContainsKey(key))
            {
                filterDictionary.Add(key, currentRecords);
            }
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

        private void AddRecordToFilterDictionaries(FileCabinetRecord record)
        {
            string dateOfBirthKey = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            AddRecordToDictionary(record, record.FirstName, this.firstNameDictionary);
            AddRecordToDictionary(record, record.LastName, this.lastNameDictionary);
            AddRecordToDictionary(record, dateOfBirthKey, this.birthdateDictionary);
        }

        private void RemoveRecord(int id)
        {
            try
            {
                List<FileCabinetRecord> recordsForDeleting;
                FileCabinetRecord record = this.records.Find(rec => rec.Id == id);

                this.records.Remove(record);

                this.firstNameDictionary.TryGetValue(record.FirstName, out recordsForDeleting);
                recordsForDeleting.Remove(record);

                this.lastNameDictionary.TryGetValue(record.LastName, out recordsForDeleting);
                recordsForDeleting.Remove(record);

                this.birthdateDictionary.TryGetValue(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), out recordsForDeleting);
                recordsForDeleting.Remove(record);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{id} doesn't exists.");
            }
        }
    }
}
