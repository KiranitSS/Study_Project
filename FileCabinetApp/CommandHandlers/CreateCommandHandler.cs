using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for record creation method.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("create", StringComparison.OrdinalIgnoreCase))
            {
                Create();
            }

            return base.Handle(request);
        }

        private static void Create()
        {
            AddRecord(Program.FileCabinetService);
        }

        private static void AddRecord(IFileCabinetService fileCabinetService)
        {
            if (fileCabinetService is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetService));
            }

            var parameters = Program.GetRecordData();
            int recId = fileCabinetService.CreateRecord(parameters);

            if (recId == -1)
            {
                Console.WriteLine($"Record is not created.");
            }
            else
            {
                Console.WriteLine($"Record #{recId} is created.");
            }
        }
    }
}
