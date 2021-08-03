using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
            };

            this.records.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            // TODO: добавьте реализацию метода
            return Array.Empty<FileCabinetRecord>();
        }

        public int GetStat()
        {
            return this.records.Count;
        }
    }
}
