using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service variation with custom settings.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Checks name for contains only letter symbols and capital letter.
        /// </summary>
        /// <param name="name">Persons name.</param>
        /// <param name="message">Name type message.</param>
        /// <returns>Are name composition is correct.</returns>
        protected static bool IsCorrectNameComposition(string name, string message)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            foreach (char symbol in name)
            {
                if (!char.IsLetter(symbol))
                {
                    Console.WriteLine($"{message} must contains only letters.");
                    return false;
                }
            }

            if (char.IsLower(name[0]))
            {
                Console.WriteLine($"{message} must starts with uppercase letter.");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool IsCorrectFirstName(string firstname)
        {
            string message = "First name";

            if (firstname is null)
            {
                Console.WriteLine($"{nameof(firstname)} can't be empty");
                return false;
            }

            if (!IsCorrectNameComposition(firstname, message))
            {
                return false;
            }

            if (firstname.Length < 2 || firstname.Length > 30)
            {
                Console.WriteLine("First name length can't be lower than 2 or bigger than 30");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool IsCorrectLastName(string lastname)
        {
            string message = "Last name";

            if (string.IsNullOrWhiteSpace(lastname))
            {
                Console.WriteLine($"{nameof(lastname)} can't be empty");
                return false;
            }

            if (!IsCorrectNameComposition(lastname, message))
            {
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
            if (dateOfBirth.Year < 1930 || dateOfBirth > DateTime.Today)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool IsCorrectMoneyCount(decimal moneyCount)
        {
            if (moneyCount < 0)
            {
                Console.WriteLine($"{nameof(moneyCount)} can't be less than zero.");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool IsCorrectPIN(short pin)
        {
            if (pin.ToString(CultureInfo.InvariantCulture).Length < 3)
            {
                Console.WriteLine("PIN length must be 3 or longer.");
                return false;
            }

            return true;
        }
    }
}
