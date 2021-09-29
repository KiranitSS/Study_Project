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
    public class EditCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("edit", StringComparison.OrdinalIgnoreCase))
            {
                Edit(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Edit(string parameters)
        {
            Console.Write("Write ID:");

            if (!TryGetId(parameters, out int id) || id > Program.FileCabinetService.GetStat())
            {
                Console.WriteLine("Incorrect ID");
                return;
            }

            var recordParameters = Program.GetRecordData();
            Program.FileCabinetService.EditRecord(id, recordParameters);
        }

        private static bool TryGetId(string input, out int id)
        {
            bool result = int.TryParse(input, out id);

            if (result && id < Program.FileCabinetService.GetStat() + 1 && id > 0)
            {
                return true;
            }

            return false;
        }
    }
}
