using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents class to read records data from xml file.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Reader with established path.</param>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads records from xml file.
        /// </summary>
        /// <returns>Returns records list.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<FileCabinetRecord>), new[] { typeof(FileCabinetRecord) });

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(this.reader))
                {
                    records = xmlSerializer.Deserialize(xmlReader) as List<FileCabinetRecord>;
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Incorrect file type.");
                records = new List<FileCabinetRecord>();
            }

            return records;
        }
    }
}
