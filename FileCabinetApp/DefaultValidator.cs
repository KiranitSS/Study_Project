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
    public class DefaultValidator : CompositeValidator
    {
        public DefaultValidator(IEnumerable<IRecordValidator> validators)
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.UtcNow),
                new MoneyCountValidator(0),
                new PinValidator(1),
                new CharPropValidator(true),
            })
        {
        }
    }
}
