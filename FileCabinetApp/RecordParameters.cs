using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains records creating parameters.
    /// </summary>>
    public class RecordParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordParameters"/> class.
        /// </summary>
        /// <param name="firstName">Firstname.</param>
        /// <param name="lastName">Lastname.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="moneyCount">Count of money.</param>
        /// <param name="pin">Security code.</param>
        /// <param name="charProp">Simple char property.</param>
        public RecordParameters(string firstName, string lastName, DateTime dateOfBirth, decimal moneyCount, short pin, char charProp)
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
