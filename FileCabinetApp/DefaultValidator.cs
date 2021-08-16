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
        /// <inheritdoc/>
        public bool IsCorrectFirstName(string firstname)
        {
            if (firstname is null)
            {
                Console.WriteLine("First name can't be empty");
                return false;
            }

            if (firstname.Length < 2 || firstname.Length > 60)
            {
                Console.WriteLine("First name length can't be lower than 2 or bigger than 60");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool IsCorrectLastName(string lastname)
        {
            if (string.IsNullOrWhiteSpace(lastname))
            {
                Console.WriteLine("Last name can't be empty");
                return false;
            }

            if (lastname.Length < 2 || lastname.Length > 60)
            {
                Console.WriteLine("Last name length can't be lower than 2 or bigger than 60");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool IsCorrectDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth.Year < 1950 || dateOfBirth > DateTime.Today)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool IsCorrectMoneyCount(decimal moneyCount)
        {
            if (moneyCount < 200)
            {
                Console.WriteLine("Count of money must be bigger than 200$");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool IsCorrectPIN(short pin)
        {
            if (pin == 0)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/> <summary>
        /// Checks ability to add simple char property.
        /// </summary>
        /// <param name="charProp">Simple char property.</param>
        /// <returns>Returns ability to add simple char property to record.</returns>
        public bool IsCorrectCharProp(char charProp)
        {
            if (charProp == default(char))
            {
                Console.WriteLine("Char prop can't be empty");
                return false;
            }

            if (!char.IsLetter(charProp))
            {
                Console.WriteLine("Char prop must be letter");
                return false;
            }

            return true;
        }
    }
}
