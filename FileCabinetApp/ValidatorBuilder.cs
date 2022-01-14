using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents builder for creating validator with defined rules.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Adds firstname validator with setted rules.
        /// </summary>
        /// <param name="min">Minimal letters count.</param>
        /// <param name="max">Maximum letters count.</param>
        /// <returns>Returns validators with one new.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds lastname validator with setted rules.
        /// </summary>
        /// <param name="min">Minimal letters count.</param>
        /// <param name="max">Maximum letters count.</param>
        /// <returns>Returns validators with one new.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds date of birth validator with setted rules.
        /// </summary>
        /// <param name="from">The earliest date of birth.</param>
        /// <param name="to">The last possible date of birth.</param>
        /// <returns>Returns validators with one new.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds money count validator with setted rules.
        /// </summary>
        /// <param name="min">Minimal count of money.</param>
        /// <returns>Returns validators with one new.</returns>
        public ValidatorBuilder ValidateMoneyCount(int min)
        {
            this.validators.Add(new MoneyCountValidator(min));
            return this;
        }

        /// <summary>
        /// Adds pin code validator with setted rules.
        /// </summary>
        /// <param name="minLength">Minimal length of pin.</param>
        /// <returns>Returns validators with one new.</returns>
        public ValidatorBuilder ValidatePin(int minLength)
        {
            this.validators.Add(new PinValidator(minLength));
            return this;
        }

        /// <summary>
        /// Adds char property validator with setted rules.
        /// </summary>
        /// <param name="mustBeLetter">Checks, is the meaning of char value must be letter.</param>
        /// <returns>Returns validators with one new.</returns>
        public ValidatorBuilder ValidateCharProp(bool mustBeLetter)
        {
            this.validators.Add(new CharPropValidator(mustBeLetter));
            return this;
        }

        /// <summary>
        /// Creates new instance of <see cref="CompositeValidator"/> from validators.
        /// </summary>
        /// <returns>Returns new instance of <see cref="CompositeValidator"/>.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
