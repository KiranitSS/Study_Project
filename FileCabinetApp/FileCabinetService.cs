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

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short shortProp, decimal money, char charProp)
        {
            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                ShortProp = shortProp,
                MoneyCount = money,
                CharProp = charProp,
            };

            this.records.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            var recordsCopy = new FileCabinetRecord[this.records.Count];

            for (int i = 0; i < this.records.Count; i++)
            {
                recordsCopy[i] = new FileCabinetRecord();
                recordsCopy[i].FirstName = this.records[i].FirstName;
                recordsCopy[i].LastName = this.records[i].LastName;
                recordsCopy[i].DateOfBirth = this.records[i].DateOfBirth;
                recordsCopy[i].ShortProp = this.records[i].ShortProp;
                recordsCopy[i].MoneyCount = this.records[i].MoneyCount;
                recordsCopy[i].CharProp = this.records[i].CharProp;
            }

            return recordsCopy;
        }

        public int GetStat()
        {
            return this.records.Count;
        }
    }
}
