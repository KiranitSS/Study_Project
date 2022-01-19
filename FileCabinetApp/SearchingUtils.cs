using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents base utils for searching records by properties.
    /// </summary>
    public static class SearchingUtils
    {
        /// <summary>
        /// Finds records by property name and value.
        /// </summary>
        /// <param name="records">Records in which the search is carried out.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        /// <returns>Indexes of the required records.<returns>
        public static List<int> FindByProp(List<FileCabinetRecord> records, string propName, string propValue)
        {
            if (propName is null)
            {
                throw new ArgumentNullException(nameof(propName));
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

        /// <summary>
        /// Gets <see cref="Dictionary{string, string}"/> with properties names as keys and their values.
        /// </summary>
        /// <param name="parameters">Input command parameters.</param>
        /// <param name="separator">Separator for splitting parameters.</param>
        /// <returns>Returns splitted data for searching records by properties.</returns>
        public static Dictionary<string, string> GetDataForFind(string parameters, string separator)
        {
            if (string.IsNullOrWhiteSpace(parameters) || separator is null)
            {
                Console.WriteLine("Incorrect parameters.");
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> searchCriteria = new Dictionary<string, string>();
            string[] searchingParameters = GetSearchingParameters(parameters, separator);

            int separatorIndex;

            foreach (var parameter in searchingParameters)
            {
                separatorIndex = parameter.IndexOf("=", StringComparison.OrdinalIgnoreCase);

                if (separatorIndex == -1)
                {
                    continue;
                }

                searchCriteria.Add(parameter[..separatorIndex].ToUpperInvariant(), parameter[(separatorIndex + 1)..]);
            }

            return searchCriteria;
        }

        private static string[] GetSearchingParameters(string parameters, string separator)
        {
            string[] updateParameters;

            if (string.IsNullOrWhiteSpace(separator))
            {
                updateParameters = parameters
                    .Replace("'", string.Empty)
                    .Replace(" ", string.Empty)
                    .Split(" ", 1);
            }
            else
            {
                updateParameters = parameters
                    .Replace("'", string.Empty)
                    .Replace(" ", string.Empty)
                    .Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }

            return updateParameters;
        }
    }
}
