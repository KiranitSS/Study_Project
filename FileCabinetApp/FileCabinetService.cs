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
    /// Represents a service for performing actions on <see cref="FileCabinetRecord"/>.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> birthdateDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">Records parameters validator.</param>
        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Add new <see cref="FileCabinetRecord"/> to records list and dictionaries.
        /// </summary>
        /// <param name="parameters">Contains creating parameters.</param>
        /// <returns> Returns ID of new record.</returns>
        public int CreateRecord(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!this.validator.ValidateParameters(parameters))
            {
                return -1;
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

            return record.Id;
        }

        /// <summary>
        /// Create copy of records.
        /// </summary>
        /// <returns>Return copy of records list.</returns>
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

        /// <summary>
        /// Returns count of records in records list.
        /// </summary>
        /// <returns>Returns records count.</returns>
        public int GetStat()
        {
            return this.records.Count;
        }

        /// <summary>
        /// Edits records properties.
        /// </summary>
        /// <param name="id">Persons ID.</param>
        /// <param name="parameters">Contains edit records parameters.</param>
        public void EditRecord(int id, RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (id > this.records.Count + 1)
            {
                throw new ArgumentException("ID can't be bigger than records count");
            }

            if (!this.validator.ValidateParameters(parameters))
            {
                Console.WriteLine("Record not editted.");
                return;
            }

            var currentRecord = this.records[id - 1];
            this.firstNameDictionary.TryGetValue(currentRecord.FirstName, out List<FileCabinetRecord> currentRecords);

            currentRecords.Remove(currentRecord);

            currentRecord.FirstName = parameters.FirstName;
            currentRecord.LastName = parameters.LastName;
            currentRecord.DateOfBirth = parameters.DateOfBirth;
            currentRecord.PIN = parameters.PIN;
            currentRecord.MoneyCount = parameters.MoneyCount;
            currentRecord.CharProp = parameters.CharProp;

            this.AddRecordToFilterDictionaries(currentRecord);
        }

        /// <summary>
        /// Searches for an entry by firstname.
        /// </summary>
        /// <param name="firstName">Search key.</param>
        /// <returns>Returns record.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(FindByKey(firstName, this.firstNameDictionary));
        }

        /// <summary>
        /// Searches for an entry by lastname.
        /// </summary>
        /// <param name="lastname">Search key.</param>
        /// <returns>>Returns record.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastname)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(FindByKey(lastname, this.lastNameDictionary));
        }

        /// <summary>
        /// Searches for an entry by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>>Returns record.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string dateOfBirth)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(FindByKey(dateOfBirth, this.birthdateDictionary));
        }

        /// <summary>
        /// Creates new record parameters validator.
        /// </summary>
        /// <returns>Returns <see cref="IRecordValidator"/> object
        /// which contains record validation settings.</returns>
        private static ReadOnlyCollection<FileCabinetRecord> FindByKey(string key, Dictionary<string, List<FileCabinetRecord>> filterDictionary)
        {
            bool contains = filterDictionary.TryGetValue(key, out List<FileCabinetRecord> currentRecords);

            if (!contains)
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            return new ReadOnlyCollection<FileCabinetRecord>(currentRecords);
        }

        private static void AddRecordToDictionary(FileCabinetRecord record, string key, Dictionary<string, List<FileCabinetRecord>> filterDictionary)
        {
            List<FileCabinetRecord> currentRecords;
            filterDictionary.TryGetValue(key, out currentRecords);

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

        private void AddRecordToFilterDictionaries(FileCabinetRecord record)
        {
            string dateOfBirthKey = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            AddRecordToDictionary(record, record.FirstName, this.firstNameDictionary);
            AddRecordToDictionary(record, record.LastName, this.lastNameDictionary);
            AddRecordToDictionary(record, dateOfBirthKey, this.birthdateDictionary);
        }
    }
}
