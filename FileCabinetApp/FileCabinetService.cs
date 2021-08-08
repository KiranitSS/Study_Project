using System;
using System.Collections.Generic;
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
    public class FileCabinetService
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> birthdateDictionary = new Dictionary<string, List<FileCabinetRecord>>();

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

            CheckCreation(parameters.FirstName, parameters.LastName, parameters.CharProp, parameters.DateOfBirth, parameters.MoneyCount);

            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                DateOfBirth = parameters.DateOfBirth,
                FiveDigitPIN = parameters.FiveDigitPIN,
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
        public FileCabinetRecord[] GetRecords()
        {
            var recordsCopy = new FileCabinetRecord[this.records.Count];

            for (int i = 0; i < this.records.Count; i++)
            {
                recordsCopy[i] = new FileCabinetRecord
                {
                    FirstName = this.records[i].FirstName,
                    LastName = this.records[i].LastName,
                    DateOfBirth = this.records[i].DateOfBirth,
                    FiveDigitPIN = this.records[i].FiveDigitPIN,
                    MoneyCount = this.records[i].MoneyCount,
                    CharProp = this.records[i].CharProp,
                    Id = this.records[i].Id,
                };
            }

            return recordsCopy;
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
            if (id > this.records.Count + 1)
            {
                throw new ArgumentException("ID can't be bigger than records count");
            }

            var currentRecord = this.records[id - 1];
            this.firstNameDictionary.TryGetValue(currentRecord.FirstName, out List<FileCabinetRecord> currentRecords);

            currentRecords.Remove(currentRecord);

            currentRecord.FirstName = parameters.FirstName;
            currentRecord.LastName = parameters.LastName;
            currentRecord.DateOfBirth = parameters.DateOfBirth;
            currentRecord.FiveDigitPIN = parameters.FiveDigitPIN;
            currentRecord.MoneyCount = parameters.MoneyCount;
            currentRecord.CharProp = parameters.CharProp;

            this.AddRecordToFilterDictionaries(currentRecord);
        }

        /// <summary>
        /// Searches for an entry by firstname.
        /// </summary>
        /// <param name="firstName">Search key.</param>
        /// <returns>Returns record.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return FindByKey(firstName, this.firstNameDictionary);
        }

        /// <summary>
        /// Searches for an entry by lastname.
        /// </summary>
        /// <param name="lastname">Search key.</param>
        /// <returns>>Returns record.</returns>
        public FileCabinetRecord[] FindByLastName(string lastname)
        {
            return FindByKey(lastname, this.lastNameDictionary);
        }

        /// <summary>
        /// Searches for an entry by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>>Returns record.</returns>
        public FileCabinetRecord[] FindByBirthDate(string dateOfBirth)
        {
            return FindByKey(dateOfBirth, this.birthdateDictionary);
        }

        private static FileCabinetRecord[] FindByKey(string key, Dictionary<string, List<FileCabinetRecord>> filterDictionary)
        {
            bool contains = filterDictionary.TryGetValue(key, out List<FileCabinetRecord> currentRecords);

            if (!contains)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return currentRecords.ToArray();
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

        private static void CheckCreation(string firstName, string lastName, char charProp, DateTime dateOfBirth, decimal fiveDigitPIN)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException($"{nameof(firstName)} can't be empty");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException($"{nameof(lastName)} can't be empty");
            }

            if (charProp == default(char))
            {
                throw new ArgumentNullException($"{nameof(charProp)} can't be empty");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name length can't be lower than 2 or bigger than 60");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("Last name length can't be lower than 2 or bigger than 60");
            }

            if (dateOfBirth.Year < 1950 || dateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("Incorrect date of birth");
            }

            if (!char.IsLetter(charProp))
            {
                throw new ArgumentException($"{nameof(charProp)} must be letter");
            }

            if (fiveDigitPIN < 200)
            {
                throw new ArgumentException($"{nameof(charProp)} must be bigger than 200");
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
