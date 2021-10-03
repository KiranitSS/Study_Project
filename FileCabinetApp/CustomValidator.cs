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
    public class CustomValidator : CompositeValidator
    {
        public CustomValidator(IEnumerable<IRecordValidator> validators)
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 40),
                new LastNameValidator(2, 20),
                new DateOfBirthValidator(new DateTime(1930, 1, 1), DateTime.UtcNow),
                new MoneyCountValidator(200),
                new PinValidator(3),
                new CharPropValidator(false),
            })
        {
        }
    }
}
