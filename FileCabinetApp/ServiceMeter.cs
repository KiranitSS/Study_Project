using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;

        public ServiceMeter(IFileCabinetService service)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            this.service = service;
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters parameters)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int id = this.service.CreateRecord(parameters);
            stopwatch.Stop();
            Console.WriteLine($"Create method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
            return id;
        }

        /// <inheritdoc/>
        public void UpdateRecords(Dictionary<string, string> paramsToChange, Dictionary<string, string> searchCriteria)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.service.UpdateRecords(paramsToChange, searchCriteria);
            stopwatch.Stop();
            Console.WriteLine($"Update method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var records = this.service.GetStat();
            stopwatch.Stop();
            Console.WriteLine($"Stat method execution duration is {stopwatch.Elapsed.Ticks} ticks.");

            return records;
        }

        /// <inheritdoc/>
        public void InsertRecord(RecordParameters parameters)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.service.InsertRecord(parameters);
            stopwatch.Stop();
            Console.WriteLine($"Insert method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void PurgeRecords()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.service.PurgeRecords();
            stopwatch.Stop();
            Console.WriteLine($"Purge method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
        }

        /// <inheritdoc/>
        public void DeleteRecords(string parameters)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.service.DeleteRecords(parameters);
            stopwatch.Stop();
            Console.WriteLine($"Delete method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot serviceSnapshot)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void SelectRecords(string parameters)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.service.SelectRecords(parameters);
            stopwatch.Stop();
            Console.WriteLine($"Select method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
        }
    }
}
