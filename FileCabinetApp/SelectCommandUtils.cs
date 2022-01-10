using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents utils class for select command.
    /// </summary>
    public static class SelectCommandUtils
    {
        /// <summary>
        /// Select and prints records data.
        /// </summary>
        /// <param name="parameters">Select command with parameters.</param>
        /// <param name="records">Records for searching and printing selected data.</param>
        public static void SelectRecordsData(string parameters, List<FileCabinetRecord> records)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Empty command parameters.");
                return;
            }

            if (records is null)
            {
                Console.WriteLine("Records are empty");
                return;
            }

            if (!TryGetCorrectDataForSelect(parameters, out Dictionary<string, string> searchingCriteria, out string[] paramsToShow))
            {
                Console.WriteLine("Incorrect parameters.");
                return;
            }

            List<FileCabinetRecord> recordsToShow = GetRecordsToShow(records, searchingCriteria, GetSeparator(parameters));

            if (recordsToShow.Count == 0)
            {
                Console.WriteLine("No such entries");
                return;
            }

            PrintRecordsAsTable(recordsToShow, new ReadOnlyCollection<string>(paramsToShow));
        }

        private static bool TryGetCorrectDataForSelect(string parameters, out Dictionary<string, string> searchingCriteria, out string[] parametersToShow)
        {
            searchingCriteria = new Dictionary<string, string>();

            parametersToShow = Array.Empty<string>();

            if (!TryGetCorrectParameters(parameters, out string[] inputs))
            {
                return false;
            }

            parametersToShow = inputs[0].Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (!AreCorrectParametersToShow(parametersToShow))
            {
                parametersToShow = Array.Empty<string>();
                return false;
            }

            parametersToShow = GetUniqueParameters(parametersToShow);

            searchingCriteria = SearchingUtils.GetDataForFind(inputs[1], GetSeparator(parameters));

            return true;
        }

        private static string[] GetUniqueParameters(string[] parametersToShow)
        {
            List<string> tmp = new List<string>();

            for (int i = 0; i < parametersToShow.Length; i++)
            {
                parametersToShow[i] = parametersToShow[i].ToUpperInvariant();
            }

            foreach (var param in parametersToShow)
            {
                if (!tmp.Contains(param))
                {
                    tmp.Add(param);
                }
            }

            parametersToShow = tmp.ToArray();
            return parametersToShow;
        }

        private static bool AreCorrectParametersToShow(string[] parametersToShow)
        {
            var properties = typeof(FileCabinetRecord).GetProperties().Select(p => p.Name).ToArray();

            foreach (var param in parametersToShow)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    if (param.Equals(properties[i], StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (i == properties.Length - 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool TryGetCorrectParameters(string parameters, out string[] inputs)
        {
            inputs = Array.Empty<string>();

            if (!parameters.Contains(" where ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            inputs = parameters.Replace(" where ", " WHERE ", StringComparison.OrdinalIgnoreCase).Split(" WHERE ");

            if (inputs.Length != 2)
            {
                return false;
            }

            inputs[0] = inputs[0].Replace(" ", string.Empty);

            if (inputs[0].Length == 0 || inputs[1].Length == 0)
            {
                return false;
            }

            return true;
        }

        private static string GetSeparator(string parameters)
        {
            if (parameters.Contains(" and ", StringComparison.OrdinalIgnoreCase))
            {
                return "and";
            }

            if (parameters.Contains(" or ", StringComparison.OrdinalIgnoreCase))
            {
                return "or";
            }

            return string.Empty;
        }

        private static List<FileCabinetRecord> GetRecordsToShow(List<FileCabinetRecord> records, Dictionary<string, string> searchingCriteria, string separator)
        {
            if (separator.Equals("or", StringComparison.OrdinalIgnoreCase))
            {
                return GetRecordsByOr(records, searchingCriteria);
            }

            if (separator.Equals("and", StringComparison.OrdinalIgnoreCase))
            {
                return GetRecordsByAnd(records, searchingCriteria);
            }

            return new List<FileCabinetRecord>();
        }

        private static List<FileCabinetRecord> GetRecordsByOr(List<FileCabinetRecord> records, Dictionary<string, string> searchingCriteria)
        {
            List<int> ids;
            List<int> indexes = new List<int>();
            List<FileCabinetRecord> recordsToShow = new List<FileCabinetRecord>();

            foreach (var criteria in searchingCriteria)
            {
                ids = SearchingUtils.FindByProp(records, criteria.Key, criteria.Value);

                for (int i = 0; i < ids.Count; i++)
                {
                    indexes.Add(records.FindIndex(r => r.Id == ids[i]));
                }

                foreach (var index in indexes)
                {
                    if (!recordsToShow.Contains(records[index]))
                    {
                        recordsToShow.Add(records[index]);
                    }
                }
            }

            return recordsToShow;
        }

        private static List<FileCabinetRecord> GetRecordsByAnd(List<FileCabinetRecord> records, Dictionary<string, string> searchingCriteria)
        {
            List<int> ids;
            List<int> indexes = new List<int>();
            List<FileCabinetRecord> recordsToShow = new List<FileCabinetRecord>();
            List<int> matchingRecordsIndexes = new List<int>();

            if (searchingCriteria.Count != 0)
            {
                ids = SearchingUtils.FindByProp(records, searchingCriteria.First().Key, searchingCriteria.First().Value);

                for (int i = 0; i < ids.Count; i++)
                {
                    matchingRecordsIndexes.Add(records.FindIndex(r => r.Id == ids[i]));
                }
            }
            else
            {
                return recordsToShow;
            }

            foreach (var criteria in searchingCriteria)
            {
                ids = SearchingUtils.FindByProp(records, criteria.Key, criteria.Value);

                for (int i = 0; i < ids.Count; i++)
                {
                    indexes.Add(records.FindIndex(r => r.Id == ids[i]));
                }

                for (int i = 0; i < matchingRecordsIndexes.Count; i++)
                {
                    if (!indexes.Contains(matchingRecordsIndexes[i]))
                    {
                        matchingRecordsIndexes.Remove(matchingRecordsIndexes[i]);
                    }
                }

                indexes.Clear();
            }

            foreach (var index in matchingRecordsIndexes)
            {
                if (!recordsToShow.Contains(records[index]))
                {
                    recordsToShow.Add(records[index]);
                }
            }

            return recordsToShow;
        }

        private static void PrintRecordsAsTable(List<FileCabinetRecord> records, ReadOnlyCollection<string> paramsToShow)
        {
            int[] columnsWidth = GetColumnsWidth(records, paramsToShow);

            StringBuilder builder = new StringBuilder();

            AddBorderLine(columnsWidth, builder);

            for (int i = 0; i < paramsToShow.Count; i++)
            {
                AddWrappedLine(builder, paramsToShow[i], columnsWidth[i]);
            }

            string[][] properties = GetPropertiesByNames(records, paramsToShow);

            AddSplitter(builder);
            builder.AppendLine();

            AddBorderLine(columnsWidth, builder);

            for (int i = 0; i < records.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    AddWrappedLine(builder, properties[j][i], columnsWidth[j]);
                }

                AddSplitter(builder);
                builder.AppendLine();
            }

            AddBorderLine(columnsWidth, builder);

            builder.AppendLine();
            Console.WriteLine(builder.ToString());
        }

        private static string[][] GetPropertiesByNames(List<FileCabinetRecord> records, ReadOnlyCollection<string> propsNames)
        {
            string[][] properties = new string[propsNames.Count][];

            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = GetPropValues(records, propsNames[i]).ToArray();
            }

            return properties;
        }

        private static string[] GetChars(List<FileCabinetRecord> records)
        {
            return records.Select(p => p.CharProp.ToString()).ToArray();
        }

        private static string[] GetPins(List<FileCabinetRecord> records)
        {
            return records.Select(p => p.PIN.ToString(CultureInfo.InvariantCulture)).ToArray();
        }

        private static string[] GetMoneyCounts(List<FileCabinetRecord> records)
        {
            return records.Select(p => p.MoneyCount.ToString(CultureInfo.InvariantCulture)).ToArray();
        }

        private static string[] GetBirthdates(List<FileCabinetRecord> records)
        {
            return records.Select(p => p.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)).ToArray();
        }

        private static string[] GetLastnames(List<FileCabinetRecord> records)
        {
            return records.Select(p => p.LastName).ToArray();
        }

        private static string[] GetFirstnames(List<FileCabinetRecord> records)
        {
            return records.Select(p => p.FirstName).ToArray();
        }

        private static string[] GetIds(List<FileCabinetRecord> records)
        {
            return records.Select(p => p.Id.ToString(CultureInfo.InvariantCulture)).ToArray();
        }

        private static int[] GetColumnsWidth(List<FileCabinetRecord> records, ReadOnlyCollection<string> parametersToShow)
        {
            int[] columnsWidths = parametersToShow.Select(p => p.Length).ToArray();

            for (int i = 0; i < columnsWidths.Length; i++)
            {
                columnsWidths[i] = GetLongest(GetPropValues(records, parametersToShow[i]), columnsWidths[i]);
            }

            return columnsWidths;
        }

        private static int GetLongest(IEnumerable<string> propValues, int headerLength)
        {
            int longestLength = propValues.Max(x => x.Length);

            return headerLength > longestLength ? headerLength : longestLength;
        }

        private static void AddBorderLine(int[] columsWidth, StringBuilder builder)
        {
            for (int i = 0; i < columsWidth.Length; i++)
            {
                AddBorder(builder, columsWidth[i]);
            }

            AddPlus(builder);
            builder.AppendLine();
        }

        private static void AddBorder(StringBuilder builder, int longestLength)
        {
            AddPlus(builder);
            AddDashes(builder, longestLength + 2);
        }

        private static void AddPlus(StringBuilder builder)
        {
            builder.Append('+');
        }

        private static void AddSplitter(StringBuilder builder)
        {
            builder.Append('|');
        }

        private static void AddDashes(StringBuilder builder, int count)
        {
            for (int i = 0; i < count; i++)
            {
                builder.Append('-');
            }
        }

        private static void AddWrappedLine(StringBuilder builder, string text, int length)
        {
            builder.Append($"| {text} ");
            int dashesCount = length - text.Length;

            for (int i = 0; i < dashesCount; i++)
            {
                builder.Append(' ');
            }
        }

        private static IEnumerable<string> GetPropValues(List<FileCabinetRecord> records, string propName)
        {
            switch (propName.ToUpperInvariant())
            {
                case "ID":
                    return GetIds(records);
                case "FIRSTNAME":
                    return GetFirstnames(records);
                case "LASTNAME":
                    return GetLastnames(records);
                case "DATEOFBIRTH":
                    return GetBirthdates(records);
                case "MONEYCOUNT":
                    return GetMoneyCounts(records);
                case "PIN":
                    return GetPins(records);
                case "CHARPROP":
                    return GetChars(records);
                default:
                    Console.WriteLine("Incorrect property name.");
                    return Array.Empty<string>();
            }
        }
    }
}
