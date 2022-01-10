using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents class to read records data from csv file.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">Reader with established path.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads records from csv file.
        /// </summary>
        /// <returns>Returns records list.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            int paramsCount = typeof(FileCabinetRecord).GetProperties().Length;

            bool isCorrectData = true;

            while (!this.reader.EndOfStream)
            {
                string[] recordData = this.reader.ReadLine().Split(",", StringSplitOptions.RemoveEmptyEntries);

                if (recordData.Length != paramsCount)
                {
                    break;
                }

                isCorrectData = TryGetRecordParameters(recordData, out RecordParameters parameters);

                if (!isCorrectData)
                {
                    continue;
                }

                records.Add(new FileCabinetRecord
                {
                    Id = parameters.Id,
                    FirstName = parameters.FirstName,
                    LastName = parameters.LastName,
                    DateOfBirth = parameters.DateOfBirth,
                    MoneyCount = parameters.MoneyCount,
                    PIN = parameters.PIN,
                    CharProp = parameters.CharProp,
                });
            }

            return records;
        }

        private static bool TryGetRecordParameters(string[] recordData, out RecordParameters parameters)
        {
            parameters = null;

            if (!int.TryParse(recordData[0], out int id))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(recordData[1]))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(recordData[2]))
            {
                return false;
            }

            if (!DateTime.TryParse(recordData[3], out DateTime dateOfBirth))
            {
                return false;
            }

            if (!decimal.TryParse(recordData[4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal moneyCount))
            {
                return false;
            }

            if (!short.TryParse(recordData[5], out short pin))
            {
                return false;
            }

            if (!char.TryParse(recordData[6], out char charProp))
            {
                return false;
            }

            parameters = new RecordParameters
            {
                Id = id,
                FirstName = recordData[1],
                LastName = recordData[2],
                DateOfBirth = dateOfBirth,
                MoneyCount = moneyCount,
                PIN = pin,
                CharProp = charProp,
            };

            return true;
        }
    }
}
