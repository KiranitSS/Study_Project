using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public abstract class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        protected CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList();
        }

        /// <summary>
        /// Validate records creation parameters.
        /// </summary>
        /// <param name="parameters">Record creation parameters.</param>
        public virtual void ValidateParameters(RecordParameters parameters)
        {
            this.validators.ForEach(v => v.ValidateParameters(parameters));
        }
    }
}
