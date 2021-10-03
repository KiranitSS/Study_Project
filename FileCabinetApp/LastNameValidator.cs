using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        public LastNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return this.ValidateLastName(parameters.LastName);
        }

        /// <summary>
        /// Checks ability to add lastname.
        /// </summary>
        /// <param name="lastname">Persons lastname.</param>
        /// <returns>Returns ability to add lastname to record.</returns>
        private bool ValidateLastName(string lastname)
        {
            if (string.IsNullOrWhiteSpace(lastname))
            {
                Console.WriteLine("Last name can't be empty");
                return false;
            }

            if (lastname.Length < this.minLength || lastname.Length > this.maxLength)
            {
                Console.WriteLine($"First name length can't be lower than {this.minLength} or bigger than {this.maxLength}");
                return false;
            }

            return true;
        }
    }
}
