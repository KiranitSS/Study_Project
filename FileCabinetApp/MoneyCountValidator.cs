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
        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.ValidateMoneyCount(parameters.MoneyCount);
        }

        /// <summary>
        /// Checks ability to add count of money.
        /// </summary>
        /// <param name="moneyCount">Persons count of money.</param>
        private void ValidateMoneyCount(decimal moneyCount)
        {
            if (moneyCount < this.min)
            {
                Console.WriteLine($"Count of money must be bigger than {this.min}$");
            }
        }
    }
}
