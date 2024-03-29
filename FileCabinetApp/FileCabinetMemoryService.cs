﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a service for actions on <see cref="FileCabinetRecord"/>.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly IRecordValidator validator;

        private string searchingResultBuffer = string.Empty;
        private string previousParameters = string.Empty;

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

            this.records.Add(record);

            if (record.Id == -1)
            {
                Console.WriteLine($"Record is not created.");
            }
            else
            {
                this.ClearBuffer();
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

            List<int> matchingIndexes = SearchingUtils.FindByProp(this.records, searchCriteria.First().Key, searchCriteria.First().Value);

            List<int> searchingIndexes;

            foreach (var critria in searchCriteria)
            {
                searchingIndexes = SearchingUtils.FindByProp(this.records, critria.Key, critria.Value);
                matchingIndexes = searchingIndexes.Intersect(matchingIndexes).ToList();
            }

            if (matchingIndexes.Count == 0)
            {
                Console.WriteLine("No any records changed.");
                return;
            }

            this.ClearBuffer();

            int recordIndex;

            foreach (var id in matchingIndexes)
            {
                recordIndex = this.records.FindIndex(x => x.Id == id);
                this.records[recordIndex] = GetUpdatedRecord(this.records[recordIndex], paramsToChange);
            }
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

            var targetIndexes = SearchingUtils.FindByProp(this.records, propName, propValue);

            if (targetIndexes.Count == 0)
            {
                Console.WriteLine("No matching entries found");
                return;
            }

            this.ClearBuffer();

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
                return;
            }

            this.ClearBuffer();

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

                this.records.Add(record);
            }
            else
            {
                var currentRecord = this.records[index];

                ReplaceRecordParameters(parameters, currentRecord);
            }
        }

        /// <inheritdoc/>
        public void SelectRecords(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Parameters can't be empty");
            }

            if (!string.IsNullOrWhiteSpace(this.previousParameters)
                && !string.IsNullOrWhiteSpace(this.searchingResultBuffer)
                && this.previousParameters.Equals(parameters, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(this.searchingResultBuffer);
            }
            else
            {
                this.previousParameters = parameters;
                this.searchingResultBuffer = SelectCommandUtils.SelectRecordsData(parameters, this.records);
            }
        }

        /// <inheritdoc/>
        public void ClearBuffer()
        {
            this.searchingResultBuffer = string.Empty;
            this.previousParameters = string.Empty;
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

        private void RemoveRecord(int id)
        {
            try
            {
                FileCabinetRecord record = this.records.Find(rec => rec.Id == id);

                this.records.Remove(record);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{id} doesn't exists.");
            }
        }
    }
}
