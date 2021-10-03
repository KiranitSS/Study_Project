using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents records validator with custom settings.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!new FirstNameValidator(2, 40).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new LastNameValidator(2, 20).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new DateOfBirthValidator(new DateTime(1930, 1, 1), DateTime.UtcNow).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new MoneyCountValidator(200).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new PinValidator(3).ValidateParameters(parameters))
            {
                return false;
            }

            if (!new CharPropValidator(false).ValidateParameters(parameters))
            {
                return false;
            }

            return true;
        }
    }
}
