using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Utils which is used get console input for records creating.
    /// </summary>
    public static class RecordConsoleDataInput
    {
        /// <summary>
        /// Fills <see cref="FileCabinetRecord"/> object properties values.
        /// </summary>
        /// <returns>Returns <see cref="FileCabinetRecord"/> object filled by console input.</returns>
        public static RecordParameters GetRecordData()
        {
            var record = new RecordParameters(
                GetAnyName("First"), GetAnyName("Last"), GetBirthDate(), GetMoney(), GetPINValue(), GetCharValue());

            return record;
        }

        /// <summary>
        /// Gets id using console input.
        /// </summary>
        /// <param name="fileCabinetService">Service from which is taken records count.</param>
        /// <returns>Returns id from console input.</returns>
        public static int GetId(FileCabinetService fileCabinetService)
        {
            if (fileCabinetService is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetService));
            }

            Console.Write("\nRecord ID: ");

            int id = -1;
            bool isCorrect = false;

            while (!isCorrect)
            {
                while (!int.TryParse(Console.ReadLine(), out id))
                {
                    Console.Write("\nWrite correct ID: ");
                }

                if (id > 0 && id < fileCabinetService.GetStat() + 1)
                {
                    isCorrect = true;
                    continue;
                }

                Console.WriteLine("Record is not found.");
                Console.Write("\nWrite correct ID: ");
            }

            return id;
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

        private static short GetPINValue()
        {
            Console.Write("\nPIN number: ");
            short anyPIN;

            while (!short.TryParse(Console.ReadLine(), out anyPIN))
            {
                Console.Write("\nWrite correct five digit PIN: ");
            }

            return anyPIN;
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
