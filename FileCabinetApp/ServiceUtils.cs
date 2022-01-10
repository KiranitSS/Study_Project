using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents base utils for operations with records.
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// Find records by property name and value.
        /// </summary>
        /// <param name="records">Records in which the search is carried out.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        /// <returns>Indexes of the required records.<returns>
        public static List<int> FindByProp(List<FileCabinetRecord> records, string propName, string propValue)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (propName is null)
            {
                throw new ArgumentNullException(nameof(propName));
            }

            if (propValue is null)
            {
                throw new ArgumentNullException(nameof(propValue));
            }

            var indexes = new List<int>();

            switch (propName.ToUpperInvariant())
            {
                case "ID":
                    indexes = records
                        .Where(rec => rec.Id
                        .ToString(CultureInfo.InvariantCulture)
                        .Equals(propValue, StringComparison.OrdinalIgnoreCase))
                        .Select(rec => rec.Id)
                        .ToList();
                    break;
                case "FIRSTNAME":
                    indexes = records
                        .Where(rec => rec.FirstName
                        .Equals(propValue, StringComparison.OrdinalIgnoreCase))
                        .Select(rec => rec.Id)
                        .ToList();
                    break;
                case "LASTNAME":
                    indexes = records
                        .Where(rec => rec.LastName
                        .Equals(propValue, StringComparison.OrdinalIgnoreCase))
                        .Select(rec => rec.Id)
                        .ToList();
                    break;
                case "DATEOFBIRTH":
                    indexes = records
                        .Where(rec => rec.DateOfBirth
                        .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                        .Equals(propValue, StringComparison.OrdinalIgnoreCase))
                        .Select(rec => rec.Id)
                        .ToList();
                    break;
                case "MONEYCOUNT":
                    indexes = records
                        .Where(rec => rec.MoneyCount
                        .ToString(CultureInfo.InvariantCulture)
                        .Equals(propValue, StringComparison.OrdinalIgnoreCase))
                        .Select(rec => rec.Id)
                        .ToList();
                    break;
                case "PIN":
                    indexes = records
                        .Where(rec => rec.PIN
                        .ToString(CultureInfo.InvariantCulture)
                        .Equals(propValue, StringComparison.OrdinalIgnoreCase))
                        .Select(rec => rec.Id)
                        .ToList();
                    break;
                case "CHARPROP":
                    indexes = records
                        .Where(rec => rec.CharProp
                        .Equals(propValue))
                        .Select(rec => rec.Id)
                        .ToList();
                    break;
                default:
                    Console.WriteLine("Incorrect property name.");
                    break;
            }

            return indexes;
        }
    }
}
