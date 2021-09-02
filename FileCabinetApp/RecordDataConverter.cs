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
        /// Initializes a new instance of the <see cref="RecordDataConverter"/> class.
        /// </summary>
        /// <param name="parameters">Record parameters.</param>
        public RecordDataConverter(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            this.Id = parameters.Id;
            this.firstName = parameters.FirstName.ToList();
            this.lastName = parameters.LastName.ToList();
            this.Year = parameters.DateOfBirth.Year;
            this.Month = parameters.DateOfBirth.Month;
            this.Day = parameters.DateOfBirth.Day;
            this.MoneyCount = parameters.MoneyCount;
            this.PIN = parameters.PIN;
            this.CharProp = parameters.CharProp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDataConverter"/> class.
        /// </summary>
        /// <param name="record">Record parameters.</param>
        public RecordDataConverter(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.Id = record.Id;
            this.firstName = record.FirstName.ToList();
            this.lastName = record.LastName.ToList();
            this.Year = record.DateOfBirth.Year;
            this.Month = record.DateOfBirth.Month;
            this.Day = record.DateOfBirth.Day;
            this.MoneyCount = record.MoneyCount;
            this.PIN = record.PIN;
            this.CharProp = record.CharProp;
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
