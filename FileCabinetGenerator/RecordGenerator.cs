using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    class RecordGenerator
    {
        private int id;

        public RecordGenerator(int startId)
        {
            this.id = startId;
        }

        public FileCabinetRecord GenerateRecord(int seedChanger)
        {
            Random rnd = new Random(DateTime.UtcNow.Millisecond + seedChanger);

            FileCabinetRecord record = new FileCabinetRecord
            {
                Id = ++this.id,
                FirstName = GenerateFirstname(rnd),
                LastName = GenerateLastname(rnd),
                DateOfBirth = GenerateDateOfBirth(rnd),
                MoneyCount = GenerateMoneyCount(rnd),
                PIN = GeneratePin(rnd),
                CharProp = GenerateChar(rnd),
            };            

            return record;
        }

        private static string GenerateFirstname(Random rnd)
        {
            return GenerateName(2, 60, rnd);
        }

        private static string GenerateLastname(Random rnd)
        {
            return GenerateName(2, 60, rnd);
        }

        private static string GenerateName(int min, int max, Random rnd)
        {
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };

            StringBuilder builder = new StringBuilder();
            int length = rnd.Next(min, max);

            for (int i = 0; i < length / 2; i++)
            {
                builder.Append(consonants[rnd.Next(consonants.Length)]);
                builder.Append(vowels[rnd.Next(vowels.Length)]);
            }

            return builder.ToString();
        }

        private static DateTime GenerateDateOfBirth(Random rnd)
        {
            return new DateTime(rnd.Next(1950, DateTime.UtcNow.Year), rnd.Next(1, 12), rnd.Next(1, 28)).AddDays(rnd.Next(3));
        }

        private static decimal GenerateMoneyCount(Random rnd)
        {
            return (decimal)(rnd.Next(200, short.MaxValue) + rnd.NextDouble()) ;
        }

        private static short GeneratePin(Random rnd)
        {
            return (short)rnd.Next(10, short.MaxValue);
        }

        private static char GenerateChar(Random rnd)
        {
            return (char)rnd.Next(65, 90);
        }
    }
}
