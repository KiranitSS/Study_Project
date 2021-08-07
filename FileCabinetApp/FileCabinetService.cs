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

            this.AddRecordToDict(record);

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

        public void EditRecord(int id)
        {
            if (id > this.records.Count + 1)
            {
                throw new ArgumentException("ID can't be bigger than records count");
            }

            var currentRecord = this.records[id - 1];
            this.firstNameDictionary.TryGetValue(currentRecord.FirstName, out List<FileCabinetRecord> currentRecords);

            currentRecords.Remove(currentRecord);

            this.records[id - 1] = RecordsUtils.GetRecordData();
            this.records[id - 1].Id = id;

            this.AddRecordToDict(this.records[id - 1]);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            bool contains = this.firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> currentRecords);

            if (!contains)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return currentRecords.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastname)
        {
            bool contains = this.lastNameDictionary.TryGetValue(lastname, out List<FileCabinetRecord> currentRecords);

            if (!contains)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return currentRecords.ToArray();
        }

        public FileCabinetRecord[] FindByBirthDate(string dateOfBirth)
        {
            bool contains = this.birthdateDictionary.TryGetValue(dateOfBirth, out List<FileCabinetRecord> currentRecords);

            if (!contains)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return currentRecords.ToArray();
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

        private void AddRecordToDict(FileCabinetRecord record)
        {
            this.AddRecordToDictByFirstname(record);
            this.AddRecordToDictByLastName(record);
            this.AddRecordToDictByBirthDate(record);
        }

        private void AddRecordToDictByFirstname(FileCabinetRecord record)
        {
            List<FileCabinetRecord> currentRecords;
            this.firstNameDictionary.TryGetValue(record.FirstName, out currentRecords);

            if (currentRecords == null)
            {
                currentRecords = new List<FileCabinetRecord>();
            }

            currentRecords.Add(record);

            if (!this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary.Add(record.FirstName, currentRecords);
            }
        }

        private void AddRecordToDictByLastName(FileCabinetRecord record)
        {
            List<FileCabinetRecord> currentRecords;
            this.lastNameDictionary.TryGetValue(record.LastName, out currentRecords);

            if (currentRecords == null)
            {
                currentRecords = new List<FileCabinetRecord>();
            }

            currentRecords.Add(record);

            if (!this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary.Add(record.LastName, currentRecords);
            }
        }

        private void AddRecordToDictByBirthDate(FileCabinetRecord record)
        {
            List<FileCabinetRecord> currentRecords;
            this.birthdateDictionary.TryGetValue(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), out currentRecords);

            if (currentRecords == null)
            {
                currentRecords = new List<FileCabinetRecord>();
            }

            currentRecords.Add(record);

            if (!this.birthdateDictionary.ContainsKey(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)))
            {
                this.birthdateDictionary.Add(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), currentRecords);
            }
        }
    }
}
