using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents <see cref="FileCabinetService"/> data to save.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records for snapshot.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Save records data to csv file.
        /// </summary>
        /// <param name="writer">Writer with established path.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException($"{nameof(writer)} is null.");
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
        /// Save records data to xml file.
        /// </summary>
        /// <param name="writer">Writer with established path.</param>
        public void SaveToXml(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException($"{nameof(writer)} is null.");
            }

            string filePath = ((FileStream)writer.BaseStream).Name;
            string fileName = GetFileName(filePath);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

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

        private static string GetFileName(string filePath)
        {
            if (filePath.Contains("\\"))
            {
                return filePath.Substring(filePath.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1);
            }

            return filePath;
        }
    }
}
