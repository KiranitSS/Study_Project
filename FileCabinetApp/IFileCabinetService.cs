﻿using System;
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

        /// <summary>
        /// Select records data and prints it.
        /// </summary>
        /// <param name="parameters">Parameters for prints records.</param>
        public void SelectRecords(string parameters);

        /// <summary>
        /// Clears searching methods buffered results.
        /// </summary>
        public void ClearBuffer();
    }
}
