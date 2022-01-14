using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents validator for verifying parameters of records.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Collection of validation rules.</param>
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
