using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class PinValidator : IRecordValidator
    {
        private readonly int minLength;

        public PinValidator(int min)
        {
            this.minLength = min;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return this.ValidatePIN(parameters.PIN);
        }

        /// <summary>
        /// Checks ability to add PIN.
        /// </summary>
        /// <param name="pin">Persons PIN code.</param>
        /// <returns>Returns ability to add PIN to record.</returns>
        private bool ValidatePIN(short pin)
        {
            if (pin.ToString(CultureInfo.InvariantCulture).Length < this.minLength)
            {
                Console.WriteLine($"PIN length must be {this.minLength} or longer.");
                return false;
            }

            return true;
        }
    }
}
