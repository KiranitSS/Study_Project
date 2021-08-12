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
            Console.Write("\nWrite any letter: ");

            while (!char.TryParse(Console.ReadLine(), out letter))
            {
                Console.Write("\nWrite correct symbol (One symbol!): ");
            }

            return letter;
        }

        private static short GetPINValue()
        {
            short pin;
            Console.Write("\nPIN number: ");

            while (!short.TryParse(Console.ReadLine(), out pin))
            {
                Console.Write("\nWrite correct PIN: ");
            }

            return pin;
        }

        private static decimal GetMoney()
        {
            decimal moneyCount;
            Console.Write("\nMoney count: ");

            while (!decimal.TryParse(Console.ReadLine(), out moneyCount))
            {
                Console.Write("\nWrite correct money count (must contains only digits): ");
            }

            return moneyCount;
        }

        private static DateTime GetBirthDate()
        {
            DateTime birth;
            Console.Write("\nDate of birth: ");

            while (!DateTime.TryParse(Console.ReadLine(), out birth))
            {
                Console.Write("\nWrite correct date (date format month/day/year): ");
            }

            return birth;
        }

        private static string GetAnyName(string message)
        {
            Console.Write($"{message} name: ");
            return Console.ReadLine();
        }
    }
}
