using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents validator with setted rules for verifying pin code.
    /// </summary>
    public class PinValidator : IRecordValidator
    {
        private readonly int minLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="PinValidator"/> class.
        /// </summary>
        /// <param name="minLength">Minimal pin code length.</param>
        public PinValidator(int minLength)
        {
            this.minLength = minLength;
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
