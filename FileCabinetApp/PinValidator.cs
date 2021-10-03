using System;
using System.Collections.Generic;
using System.Globalization;
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
        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.ValidatePIN(parameters.PIN);
        }

        /// <summary>
        /// Checks ability to add PIN.
        /// </summary>
        /// <param name="pin">Persons PIN code.</param>
        private void ValidatePIN(short pin)
        {
            if (pin.ToString(CultureInfo.InvariantCulture).Length < this.minLength)
            {
                Console.WriteLine($"PIN length must be {this.minLength} or longer.");
            }
        }
    }
}
