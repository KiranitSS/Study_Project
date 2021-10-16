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
        public void EditRecord(int id, RecordParameters parameters)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.service.EditRecord(id, parameters);
            stopwatch.Stop();
            Console.WriteLine($"Edit method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string dateOfBirth)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = this.service.FindByBirthDate(dateOfBirth);
            stopwatch.Stop();
            Console.WriteLine($"Find method execution duration is {stopwatch.Elapsed.Ticks} ticks.");

            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = this.service.FindByFirstName(firstName);
            stopwatch.Stop();
            Console.WriteLine($"Find method execution duration is {stopwatch.Elapsed.Ticks} ticks.");

            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastname)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = this.service.FindByLastName(lastname);
            stopwatch.Stop();
            Console.WriteLine($"Find method execution duration is {stopwatch.Elapsed.Ticks} ticks.");

            return result;
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
        public void RemoveRecord(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.service.RemoveRecord(id);
            stopwatch.Stop();
            Console.WriteLine($"Remove method execution duration is {stopwatch.Elapsed.Ticks} ticks.");
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot serviceSnapshot)
        {
            throw new NotSupportedException();
        }
    }
}
