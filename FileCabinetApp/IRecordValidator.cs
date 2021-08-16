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
        /// Checks ability to add firstname.
        /// </summary>
        /// <param name="firstname">Persons firstname.</param>
        /// <returns>Returns ability to add firstname to record.</returns>
        public bool IsCorrectFirstName(string firstname);

        /// <summary>
        /// Checks ability to add lastname.
        /// </summary>
        /// <param name="lastname">Persons lastname.</param>
        /// <returns>Returns ability to add lastname to record.</returns>
        public bool IsCorrectLastName(string lastname);

        /// <summary>
        /// Checks ability to add date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Persons date of birth.</param>
        /// <returns>Returns ability to add date of birth to record.</returns>
        public bool IsCorrectDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Checks ability to add count of money.
        /// </summary>
        /// <param name="moneyCount">Persons count of money.</param>
        /// <returns>Returns ability to add count of money to record.</returns>
        public bool IsCorrectMoneyCount(decimal moneyCount);

        /// <summary>
        /// Checks ability to add PIN.
        /// </summary>
        /// <param name="pin">Persons PIN code.</param>
        /// <returns>Returns ability to add PIN to record.</returns>
        public bool IsCorrectPIN(short pin);

        /// <summary>
        /// Checks ability to add simple char property.
        /// </summary>
        /// <param name="charProp">Simple char property.</param>
        /// <returns>Returns ability to add simple char property to record.</returns>
        public bool IsCorrectCharProp(char charProp);
    }
}
