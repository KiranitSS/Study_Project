using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service variation with default settings.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <inheritdoc/>
        protected override bool IsCorrectFirstName(string firstname)
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

        /// <inheritdoc/>
        protected override bool IsCorrectLastName(string lastname)
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

        /// <inheritdoc/>
        protected override bool IsCorrectDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth.Year < 1950 || dateOfBirth > DateTime.Today)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool IsCorrectMoneyCount(decimal moneyCount)
        {
            if (moneyCount < 200)
            {
                Console.WriteLine($"{nameof(moneyCount)} must be bigger than 200$");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool IsCorrectCharProp(char charProp)
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
