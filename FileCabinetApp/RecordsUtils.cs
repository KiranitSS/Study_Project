using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public static class RecordsUtils
    {
        public static void AddRecord(FileCabinetService fileCabinetService)
        {
            if (fileCabinetService is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetService));
            }

            FileCabinetRecord record = GetRecordData();

            int recId = fileCabinetService.CreateRecord(
                record.FirstName, record.LastName, record.DateOfBirth, record.ShortProp, record.MoneyCount, record.CharProp);

            Console.WriteLine($"Record #{recId} is created.");
        }

        public static FileCabinetRecord GetRecordData()
        {
            FileCabinetRecord record = new FileCabinetRecord(
                GetAnyName("First"), GetAnyName("Last"), GetBirthDate(), GetMoney(), GetShortValue(), GetCharValue());

            return record;
        }

        private static char GetCharValue()
        {
            char letter = default(char);
            bool isCorrect = false;

            while (!isCorrect)
            {
                Console.Write("\nWrite any letter: ");
                while (!char.TryParse(Console.ReadLine(), out letter))
                {
                    Console.Write("\nWrite correct symbol (One symbol!): ");
                }

                if (char.IsLetter(letter))
                {
                    isCorrect = true;
                }
            }

            return letter;
        }

        private static short GetShortValue()
        {
            Console.Write("\nShort number: ");
            short anyShort;

            while (!short.TryParse(Console.ReadLine(), out anyShort))
            {
                Console.Write("\nWrite correct short number: ");
            }

            return anyShort;
        }

        private static decimal GetMoney()
        {
            decimal moneyCount = 0;
            bool isCorrect = false;

            while (!isCorrect)
            {
                Console.Write("\nMoney count: ");
                while (!decimal.TryParse(Console.ReadLine(), out moneyCount))
                {
                    Console.Write("\nWrite correct money count (must contains only digits): ");
                }

                if (moneyCount > 200)
                {
                    isCorrect = true;
                    continue;
                }

                Console.WriteLine("Money count must be bigger than 200!");
            }

            return moneyCount;
        }

        private static DateTime GetBirthDate()
        {
            DateTime birth = DateTime.Now;
            bool isCorrect = false;

            while (!isCorrect)
            {
                Console.Write("\nDate of birth: ");
                while (!DateTime.TryParse(Console.ReadLine(), out birth))
                {
                    Console.Write("\nWrite correct date (date format month/day/year): ");
                }

                if (birth < DateTime.Today && birth.Year > 1950)
                {
                    isCorrect = true;
                    continue;
                }

                Console.WriteLine("Write your real date of birth!");
            }

            return birth;
        }

        private static string GetAnyName(string message)
        {
            string name;
            do
            {
                Console.Write($"{message} name: ");
                name = Console.ReadLine();
            }
            while (name.Length < 2 || name.Length > 60);
            return name;
        }
    }
}
