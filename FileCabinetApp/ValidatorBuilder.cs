using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        public ValidatorBuilder ValidateMoneyCount(int min)
        {
            this.validators.Add(new MoneyCountValidator(min));
            return this;
        }

        public ValidatorBuilder ValidatePin(int minLength)
        {
            this.validators.Add(new PinValidator(minLength));
            return this;
        }

        public ValidatorBuilder ValidateCharProp(bool mustBeLetter)
        {
            this.validators.Add(new CharPropValidator(mustBeLetter));
            return this;
        }

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
