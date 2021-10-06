using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return this.ValidateDateOfBirth(parameters.DateOfBirth);
        }

        /// <summary>
        /// Checks ability to add date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Persons date of birth.</param>
        private bool ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < this.from || dateOfBirth > this.to)
            {
                Console.WriteLine("Incorrect date of birth");
                return false;
            }

            return true;
        }
    }
}
