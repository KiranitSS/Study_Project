using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly string logPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            this.service = service;
            this.logPath = "log.txt";
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling Create() with FirstName = '{parameters.FirstName}', LastName = '{parameters.LastName}'," +
                    $" DateOfBirth = '{parameters.DateOfBirth}', MoneyCount = '{parameters.MoneyCount}'," +
                    $" Pin = '{parameters.PIN}', CharProp = '{parameters.CharProp}'";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            return this.service.CreateRecord(parameters);
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling Edit() with Id = '{id}, 'FirstName = '{parameters.FirstName}', LastName = '{parameters.LastName}'," +
                                $" DateOfBirth = '{parameters.DateOfBirth}', MoneyCount = '{parameters.MoneyCount}'," +
                                $" Pin = '{parameters.PIN}', CharProp = '{parameters.CharProp}'";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            this.service.EditRecord(id, parameters);
        }

        /// <inheritdoc/>
        public void UpdateRecords(Dictionary<string, string> paramsToChange, Dictionary<string, string> paramsByWhichChange)
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling Update() with paramsToChange '{paramsToChange}', paramsByWhichChange '{paramsByWhichChange}'";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            this.service.UpdateRecords(paramsToChange, paramsByWhichChange);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling FindByFirstName() with LastName '{firstName}'";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            return this.service.FindByFirstName(firstName);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastname)
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling FindByLastName() with LastName '{lastname}'";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            return this.service.FindByLastName(lastname);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string dateOfBirth)
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling FindByBirthDate() with DateOfBirth '{dateOfBirth}'";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            return this.service.FindByBirthDate(dateOfBirth);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling GetStat() without any parameters.";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            return this.service.GetStat();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void PurgeRecords()
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling Purge() without any parameters.";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            this.service.PurgeRecords();
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling Remove() with Id '{id}'";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            this.service.RemoveRecord(id);
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot serviceSnapshot)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void InsertRecord(RecordParameters parameters)
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling Insert() with Parameters '{parameters}'.";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            this.service.InsertRecord(parameters);
        }

        /// <inheritdoc/>
        public void DeleteRecords(string parameters)
        {
            using (StreamWriter writer = new StreamWriter(this.logPath, true))
            {
                string msg = $"{DateTime.Now} - Calling Delete() with Parameters '{parameters}'.";

                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.Flush();
            }

            this.service.DeleteRecords(parameters);
        }
    }
}
