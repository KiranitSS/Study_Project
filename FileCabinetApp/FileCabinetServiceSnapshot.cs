using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents <see cref="FileCabinetMemoryService"/> data to save.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records for snapshot.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <value>
        /// Records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get => new (this.records); }

        /// <summary>
        /// Saves records data to csv file.
        /// </summary>
        /// <param name="writer">Writer with established path.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            string filePath = ((FileStream)writer.BaseStream).Name;

            string fileName = GetFileName(filePath);

            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);

            var props = typeof(FileCabinetRecord).GetProperties();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < props.Length - 1; i++)
            {
                builder.Append(props[i].Name);
                builder.Append(',');
            }

            builder.Append(props[^1].Name);

            writer.WriteLine(builder);

            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i]);
            }

            Console.WriteLine($"All records are exported to file {fileName}");
        }

        /// <summary>
        /// Saves records data to xml file.
        /// </summary>
        /// <param name="writer">Writer with established path.</param>
        public void SaveToXml(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            string filePath = ((FileStream)writer.BaseStream).Name;
            string fileName = GetFileName(filePath);

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
            };

            var xmlWriter = XmlWriter.Create(writer, settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("records");

            FileCabinetRecordXmlWriter recordXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);

            for (int i = 0; i < this.records.Length; i++)
            {
                recordXmlWriter.Write(this.records[i]);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();

            xmlWriter.Close();

            Console.WriteLine($"All records are exported to file {fileName}");
        }

        /// <summary>
        /// Loads records from csv file.
        /// </summary>
        /// <param name="reader">Reader with established path.</param>
        /// <param name="validator">Checks records correctness.</param>
        public void LoadFromCsv(StreamReader reader, IRecordValidator validator)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(reader);
            List<FileCabinetRecord> importedRecords = csvReader.ReadAll().ToList();

            importedRecords = RemoveIncorrectRecords(importedRecords, validator);

            this.RemoveDuplicatedRecords(importedRecords);
        }

        private static bool IsValidRecord(FileCabinetRecord record, IRecordValidator validator)
        {
            if (!validator.IsCorrectFirstName(record.FirstName))
            {
                return false;
            }

            if (!validator.IsCorrectLastName(record.LastName))
            {
                return false;
            }

            if (!validator.IsCorrectDateOfBirth(record.DateOfBirth))
            {
                return false;
            }

            if (!validator.IsCorrectMoneyCount(record.MoneyCount))
            {
                return false;
            }

            if (!validator.IsCorrectPIN(record.PIN))
            {
                return false;
            }

            if (!validator.IsCorrectCharProp(record.CharProp))
            {
                return false;
            }

            return true;
        }

        private static List<FileCabinetRecord> RemoveIncorrectRecords(List<FileCabinetRecord> importedRecords, IRecordValidator validator)
        {
            List<FileCabinetRecord> validRecords = importedRecords.ToList();

            foreach (var record in importedRecords)
            {
                if (!IsValidRecord(record, validator))
                {
                    Console.WriteLine($"Record with Id {record.Id}, is incorrect.");
                    validRecords.Remove(record);
                }
            }

            return validRecords;
        }

        private static string GetFileName(string filePath)
        {
            if (filePath.Contains("\\"))
            {
                return filePath[(filePath.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1) ..];
            }

            return filePath;
        }

        private void RemoveDuplicatedRecords(List<FileCabinetRecord> importedRecords)
        {
            var indexes = importedRecords.Select(rec => rec.Id).ToList();

            List<FileCabinetRecord> uniqueRecords = this.records.Where(rec => !indexes.Contains(rec.Id)).ToList();

            uniqueRecords.AddRange(importedRecords);

            this.records = uniqueRecords.ToArray();
        }
    }
}
