using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents records validation modes.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validate records creation parameters.
        /// </summary>
        /// <param name="parameters">Record creation parameters.</param>
        /// <returns>Returns is record parameters valid.</returns>
        public bool ValidateParameters(RecordParameters parameters);
    }
}
