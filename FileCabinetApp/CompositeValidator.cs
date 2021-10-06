using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList();
        }

        /// <inheritdoc/>
        public virtual bool ValidateParameters(RecordParameters parameters)
        {
            foreach (var validator in this.validators)
            {
                if (!validator.ValidateParameters(parameters))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
