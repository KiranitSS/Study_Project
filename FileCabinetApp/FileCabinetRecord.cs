using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents an <see cref="FileCabinetRecord"/> object for saving persons data and perform actions on it.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        public FileCabinetRecord() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="firstName">Persons firstname.</param>
        /// <param name="lastName">Persons lastname.</param>
        /// <param name="dateOfBirth">Persons date of birth.</param>
        /// <param name="moneyCount">Count of persons money.</param>
        /// <param name="pin">Security money code.</param>
        /// <param name="charProp">Simple char prop.</param>
        public FileCabinetRecord(string firstName, string lastName, DateTime dateOfBirth, decimal moneyCount, short pin, char charProp)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.MoneyCount = moneyCount;
            this.PIN = pin;
            this.CharProp = charProp;
        }

        /// <summary>
        /// Gets or sets persons ID.
        /// </summary>
        /// <value>
        /// Persons ID.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets persons firstname.
        /// </summary>
        /// <value>
        /// Persons firstname.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets persons lastname.
        /// </summary>
        /// <value>
        /// Persons lastname.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets persons date of birth.
        /// </summary>
        /// <value>
        /// Persons date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

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
    }
}
