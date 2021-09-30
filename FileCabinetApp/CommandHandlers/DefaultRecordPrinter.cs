using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class DefaultRecordPrinter : IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            records.ToList().ForEach(rec => Console.WriteLine($"#{rec.Id}, {rec.FirstName}, " +
                $"{rec.LastName}, {rec.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{rec.PIN}, {rec.MoneyCount}$, {rec.CharProp}"));
        }
    }
}
