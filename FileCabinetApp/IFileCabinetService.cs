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
        /// /// <param name="parameters">Contains creating parameters.</param>
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

        /// <summary>
        /// Restores records.
        /// </summary>
        /// <param name="serviceSnapshot">Service which contains last records state.</param>
        public void Restore(FileCabinetServiceSnapshot serviceSnapshot);

        /// <summary>
        /// Creates <see cref="FileCabinetServiceSnapshot"/> instance with avaible now records.
        /// </summary>
        /// <returns>Returns <see cref="FileCabinetServiceSnapshot"/> instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Removes record from records list.
        /// </summary>
        /// <param name="id">ID of the record to delete.</param>
        public void RemoveRecord(int id);

        /// <summary>
        /// Purges removed records from storage.
        /// </summary>
        public void PurgeRecords();
    }
}
