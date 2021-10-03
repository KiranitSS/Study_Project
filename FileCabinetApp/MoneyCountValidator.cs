using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class MoneyCountValidator : IRecordValidator
    {
        private readonly int min;

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
        /// <returns>Returns ability to add count of money to record.</returns>
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
