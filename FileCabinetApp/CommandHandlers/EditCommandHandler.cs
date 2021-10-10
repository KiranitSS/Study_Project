using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for record edition method.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public EditCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("edit", StringComparison.OrdinalIgnoreCase))
            {
                this.Edit(request.Parameters);
            }

            return base.Handle(request);
        }

        private void Edit(string parameters)
        {
            if (!this.TryGetId(parameters, out int id) || id > this.Service.GetStat())
            {
                Console.WriteLine("Incorrect ID");
                return;
            }

            var recordParameters = Program.GetRecordData();

            if (Program.IsLogging)
            {
                new ServiceLogger(new ServiceMeter(this.Service)).EditRecord(id, recordParameters);
            }
            else
            {
                new ServiceMeter(this.Service).EditRecord(id, recordParameters);
            }
        }

        private bool TryGetId(string input, out int id)
        {
            bool result = int.TryParse(input, out id);

            if (result && id < this.Service.GetStat() + 1 && id > 0)
            {
                return true;
            }

            return false;
        }
    }
}
