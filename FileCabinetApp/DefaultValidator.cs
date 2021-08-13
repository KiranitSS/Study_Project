using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents records validator with default settings.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Checks abillity to create a record.
        /// </summary>
        /// <param name="parameters">Contains creating parameters.</param>
        /// <returns>Returns possibility or impossibility to create record.</returns>
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                Console.WriteLine("Parameters empty");
                return false;
            }

            if (!IsCorrectFirstName(parameters.FirstName))
            {
                Console.WriteLine("Incorrect firstname!");
                return false;
            }

            if (!IsCorrectLastName(parameters.LastName))
            {
                Console.WriteLine("Incorrect lastname!");
                return false;
            }

            if (!IsCorrectDateOfBirth(parameters.DateOfBirth))
            {
                Console.WriteLine("Incorrect date of birth!");
                return false;
            }

            if (!IsCorrectMoneyCount(parameters.MoneyCount))
            {
                Console.WriteLine("Incorrect count of money!");
                return false;
            }

            if (!IsCorrectCharProp(parameters.CharProp))
            {
                Console.WriteLine("Incorrect count of money!");
                return false;
            }

            return true;
        }

        private static bool IsCorrectFirstName(string firstname)
        {
            if (firstname is null)
            {
                Console.WriteLine($"{nameof(firstname)} can't be empty");
                return false;
            }

            if (firstname.Length < 2 || firstname.Length > 60)
            {
                Console.WriteLine("First name length can't be lower than 2 or bigger than 60");
                return false;
            }

            return true;
        }

        private static bool IsCorrectLastName(string lastname)
        {
            if (string.IsNullOrWhiteSpace(lastname))
            {
                Console.WriteLine($"{nameof(lastname)} can't be empty");
                return false;
            }

            if (lastname.Length < 2 || lastname.Length > 60)
            {
                Console.WriteLine("First name length can't be lower than 2 or bigger than 60");
                return false;
            }

            return true;
        }

        private static bool IsCorrectDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth.Year < 1950 || dateOfBirth > DateTime.Today)
            {
                return false;
            }

            return true;
        }

        private static bool IsCorrectMoneyCount(decimal moneyCount)
        {
            if (moneyCount < 200)
            {
                Console.WriteLine($"{nameof(moneyCount)} must be bigger than 200$");
                return false;
            }

            return true;
        }

        private static bool IsCorrectCharProp(char charProp)
        {
            if (charProp == default(char))
            {
                Console.WriteLine($"{nameof(charProp)} can't be empty");
                return false;
            }

            if (!char.IsLetter(charProp))
            {
                Console.WriteLine($"{nameof(charProp)} must be letter");
                return false;
            }

            return true;
        }
    }
}
