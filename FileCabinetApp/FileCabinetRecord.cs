using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public FileCabinetRecord() { }

        public FileCabinetRecord(string firstName, string lastName, DateTime dateOfBirth, decimal moneyCount, short shortProp, char charProp)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.MoneyCount = moneyCount;
            this.ShortProp = shortProp;
            this.CharProp = charProp;
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public decimal MoneyCount { get; set; }

        public short ShortProp { get; set; }

        public char CharProp { get; set; }
    }
}
