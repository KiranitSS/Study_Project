using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a service for actions on records.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Add new <see cref="FileCabinetRecord"/> to records list and dictionaries.
        /// </summary>
        /// <param name="parameters">Contains creating parameters.</param>
        /// <returns> Returns ID of new record.</returns>
        public int CreateRecord(RecordParameters parameters);

        /// <summary>
        /// Edits records properties.
        /// </summary>
        /// <param name="id">Persons ID.</param>
        /// <param name="parameters">Contains edit records parameters.</param>
        public void EditRecord(int id, RecordParameters parameters);

        /// <summary>
        /// Create copy of records.
        /// </summary>
        /// <returns>Return copy of records list.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns count of records in records list.
        /// </summary>
        /// <returns>Returns records count.</returns>
        public int GetStat();

        /// <summary>
        /// Searches for an entry by firstname.
        /// </summary>
        /// <param name="firstName">Search key.</param>
        /// <returns>Returns record.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches for an entry by lastname.
        /// </summary>
        /// <param name="lastname">Search key.</param>
        /// <returns>>Returns record.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastname);

        /// <summary>
        /// Searches for an entry by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>>Returns record.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string dateOfBirth);
    }
}
