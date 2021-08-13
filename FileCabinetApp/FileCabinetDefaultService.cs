using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service variation with default settings.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <inheritdoc/>
        protected override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
