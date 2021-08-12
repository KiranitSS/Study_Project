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

            if (!this.ValidateParameters(parameters))
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
                    PIN = this.records[i].PIN,
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
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

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

        /// <summary>
        /// Checks abillity to create a record.
        /// </summary>
        /// <param name="parameters">Contains creating parameters.</param>
        /// <returns>Returns possibility or impossibility to create record.</returns>
        protected bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                Console.WriteLine("Parameters empty");
                return false;
            }

            if (!this.IsCorrectFirstName(parameters.FirstName))
            {
                Console.WriteLine("Incorrect firstname!");
                return false;
            }

            if (!this.IsCorrectLastName(parameters.LastName))
            {
                Console.WriteLine("Incorrect lastname!");
                return false;
            }

            if (!this.IsCorrectDateOfBirth(parameters.DateOfBirth))
            {
                Console.WriteLine("Incorrect date of birth!");
                return false;
            }

            if (!this.IsCorrectMoneyCount(parameters.MoneyCount))
            {
                Console.WriteLine("Incorrect count of money!");
                return false;
            }

            if (!this.IsCorrectPIN(parameters.PIN))
            {
                Console.WriteLine("Incorrect PIN!");
                return false;
            }

            if (!this.IsCorrectCharProp(parameters.CharProp))
            {
                Console.WriteLine("Incorrect count of money!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks ability to add firstname.
        /// </summary>
        /// <param name="firstname">Persons firstname.</param>
        /// <returns>Returns ability to add firstname to record.</returns>
        protected virtual bool IsCorrectFirstName(string firstname)
        {
            if (string.IsNullOrWhiteSpace(firstname))
            {
                Console.WriteLine($"{nameof(firstname)} can't be empty");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks ability to add lastname.
        /// </summary>
        /// <param name="lastname">Persons lastname.</param>
        /// <returns>Returns ability to add lastname to record.</returns>
        protected virtual bool IsCorrectLastName(string lastname)
        {
            if (string.IsNullOrWhiteSpace(lastname))
            {
                Console.WriteLine($"{nameof(lastname)} can't be empty");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks ability to add date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Persons date of birth.</param>
        /// <returns>Returns ability to add date of birth to record.</returns>
        protected virtual bool IsCorrectDateOfBirth(DateTime dateOfBirth)
        {
            return true;
        }

        /// <summary>
        /// Checks ability to add count of money.
        /// </summary>
        /// <param name="moneyCount">Persons count of money.</param>
        /// <returns>Returns ability to add count of money to record.</returns>
        protected virtual bool IsCorrectMoneyCount(decimal moneyCount)
        {
            return true;
        }

        /// <summary>
        /// Checks ability to add PIN.
        /// </summary>
        /// <param name="pin">Persons PIN code.</param>
        /// <returns>Returns ability to add PIN to record.</returns>
        protected virtual bool IsCorrectPIN(short pin)
        {
            return true;
        }

        /// <summary>
        /// Checks ability to add simple char property.
        /// </summary>
        /// <param name="charProp">Simple char property.</param>
        /// <returns>Returns ability to add simple char property to record.</returns>
        protected virtual bool IsCorrectCharProp(char charProp)
        {
            return true;
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

        private void AddRecordToFilterDictionaries(FileCabinetRecord record)
        {
            string dateOfBirthKey = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            AddRecordToDictionary(record, record.FirstName, this.firstNameDictionary);
            AddRecordToDictionary(record, record.LastName, this.lastNameDictionary);
            AddRecordToDictionary(record, dateOfBirthKey, this.birthdateDictionary);
        }
    }
}
