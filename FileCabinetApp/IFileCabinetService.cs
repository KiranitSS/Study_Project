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
        /// Updates records properties.
        /// </summary>
        /// <param name="paramsToChange">Records params which would be changed.</param>
        /// <param name="searchCriteria">Params by which record would be founded.</param>
        public void UpdateRecords(Dictionary<string, string> paramsToChange, Dictionary<string, string> searchCriteria);

        /// <summary>
        /// Create copy of all records.
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
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches for an entry by lastname.
        /// </summary>
        /// <param name="lastname">Search key.</param>
        /// <returns>>Returns record.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastname);

        /// <summary>
        /// Searches for an entry by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Search key.</param>
        /// <returns>>Returns record.</returns>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string dateOfBirth);

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
        /// Deletes record from records list.
        /// </summary>
        /// <param name="parameters">Parameters of the record to delete.</param>
        public void DeleteRecords(string parameters);

        /// <summary>
        /// Purges removed records from storage.
        /// </summary>
        public void PurgeRecords();

        /// <summary>
        /// Insert record in records list.
        /// </summary>
        /// <param name="parameters">Record for insert.</param>
        public void InsertRecord(RecordParameters parameters);
    }
}
