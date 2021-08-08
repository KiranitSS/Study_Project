using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(Comparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> birthdateDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short shortProp, decimal money, char charProp)
        {
            CheckCreation(firstName, lastName, charProp, dateOfBirth, money);

            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                ShortProp = shortProp,
                MoneyCount = money,
                CharProp = charProp,
            };

            this.AddRecordToFilterDictionaries(record);

            this.records.Add(record);

            return record.Id;
        }

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
                    ShortProp = this.records[i].ShortProp,
                    MoneyCount = this.records[i].MoneyCount,
                    CharProp = this.records[i].CharProp,
                    Id = this.records[i].Id,
                };
            }

            return recordsCopy;
        }

        public int GetStat()
        {
            return this.records.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short shortProp, decimal money, char charProp)
        {
            if (id > this.records.Count + 1)
            {
                throw new ArgumentException("ID can't be bigger than records count");
            }

            var currentRecord = this.records[id - 1];
            this.firstNameDictionary.TryGetValue(currentRecord.FirstName, out List<FileCabinetRecord> currentRecords);

            currentRecords.Remove(currentRecord);

            currentRecord.FirstName = firstName;
            currentRecord.LastName = lastName;
            currentRecord.DateOfBirth = dateOfBirth;
            currentRecord.ShortProp = shortProp;
            currentRecord.MoneyCount = money;
            currentRecord.CharProp = charProp;

            this.AddRecordToFilterDictionaries(currentRecord);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return FindByKey(firstName, this.firstNameDictionary);
        }

        public FileCabinetRecord[] FindByLastName(string lastname)
        {
            return FindByKey(lastname, this.lastNameDictionary);
        }

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

        private static void CheckCreation(string firstName, string lastName, char charProp, DateTime dateOfBirth, decimal shortProp)
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

            if (shortProp < 200)
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
