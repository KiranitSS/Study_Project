using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Converts records data into a suitable format for saving.
    /// </summary>
    [Serializable]
    public class RecordDataConverter
    {
        private List<char> firstName;
        private List<char> lastName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDataConverter"/> class.
        /// </summary>
        public RecordDataConverter()
        {
        }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// Persons status.
        /// </value>
        public short Status { get; set; }

        /// <summary>
        /// Gets or sets persons ID.
        /// </summary>
        /// <value>
        /// Persons ID.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets persons year of birth.
        /// </summary>
        /// <value>
        /// Persons year of birth.
        /// </value>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets persons month of birth.
        /// </summary>
        /// <value>
        /// Persons month of birth.
        /// </value>
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets persons day of birth.
        /// </summary>
        /// <value>
        /// Persons day of birth.
        /// </value>
        public int Day { get; set; }

        /// <summary>
        /// Gets or sets count of persons money.
        /// </summary>
        /// <value>
        /// Count of persons money.
        /// </value>
        public decimal MoneyCount { get; set; }

        /// <summary>
        /// Gets or sets persons security code.
        /// </summary>
        /// <value>
        /// Persons security code.
        /// </value>
        public short PIN { get; set; }

        /// <summary>
        /// Gets or sets simple char prop.
        /// </summary>
        /// <value>
        /// Simple char prop.
        /// </value>
        public char CharProp { get; set; }

        /// <summary>
        /// Gets persons firstname.
        /// </summary>
        /// <returns>
        /// Persons firstname.
        /// </returns>
        public List<char> GetFirstName()
        {
            return this.firstName;
        }

        /// <summary>
        /// Gets persons firstname.
        /// </summary>
        /// <param name="value">
        /// Persons firstname.
        /// </param>
        public void SetFirstName(List<char> value)
        {
            this.firstName = value;
        }

        /// <summary>
        /// Gets persons lastName.
        /// </summary>
        /// <returns>
        /// Persons lastName.
        /// </returns>
        public List<char> GetLastName()
        {
            return this.lastName;
        }

        /// <summary>
        /// Gets persons lastName.
        /// </summary>
        /// <param name="value">
        /// Persons lastName.
        /// </param>
        public void SetLastName(List<char> value)
        {
            this.lastName = value;
        }
    }
}
