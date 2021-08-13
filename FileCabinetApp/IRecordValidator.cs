using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents validation modes.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Checks abillity to create a record.
        /// </summary>
        /// <param name="parameters">Contains creating parameters.</param>
        /// <returns>Returns possibility or impossibility to create record.</returns>
        public bool ValidateParameters(RecordParameters parameters);
    }
}
