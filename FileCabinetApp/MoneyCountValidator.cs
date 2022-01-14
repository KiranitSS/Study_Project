using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents validator with setted rules for verifying money count.
    /// </summary>
    public class MoneyCountValidator : IRecordValidator
    {
        private readonly int min;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyCountValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal count of money.</param>
        public MoneyCountValidator(int min)
        {
            this.min = min;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return this.ValidateMoneyCount(parameters.MoneyCount);
        }

        /// <summary>
        /// Checks ability to add count of money.
        /// </summary>
        /// <param name="moneyCount">Persons count of money.</param>
        private bool ValidateMoneyCount(decimal moneyCount)
        {
            if (moneyCount < this.min)
            {
                Console.WriteLine($"Count of money must be bigger than {this.min}$");
                return false;
            }

            return true;
        }
    }
}
