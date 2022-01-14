using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents validator with setted rules for verifying firstname.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal letters count.</param>
        /// <param name="max">Maximum letters count.</param>
        public FirstNameValidator(int min, int max)
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

            return this.ValidateFirstName(parameters.FirstName);
        }

        /// <summary>
        /// Checks ability to add firstname.
        /// </summary>
        /// <param name="firstname">Persons firstname.</param>
        private bool ValidateFirstName(string firstname)
        {
            if (firstname is null)
            {
                Console.WriteLine("First name can't be empty");
                return false;
            }

            if (firstname.Length < this.minLength || firstname.Length > this.maxLength)
            {
                Console.WriteLine($"First name length can't be lower than {this.minLength} or bigger than {this.maxLength}");
                return false;
            }

            return true;
        }
    }
}
