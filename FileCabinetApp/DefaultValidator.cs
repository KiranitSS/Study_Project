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
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!new FirstNameValidator(2, 60).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new LastNameValidator(2, 60).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.UtcNow).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new MoneyCountValidator(0).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new PinValidator(0).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new CharPropValidator(true).ValidateParameters(parameters))
            {
                return false;
            }

            return true;
        }
    }
}
