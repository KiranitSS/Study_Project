using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for deleting records.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                this.Delete(request.Parameters);
            }

            return base.Handle(request);
        }

        private static bool AreCorrectDeletionParameters(string parameters)
        {
            if (parameters.IndexOf("where", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            parameters = parameters.Replace("where", string.Empty).TrimStart();

            if (string.IsNullOrEmpty(parameters) || !parameters.Contains('=') || parameters.Length < 3)
            {
                return false;
            }

            return true;
        }

        private void Delete(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Parameters can't be empty.");
                return;
            }

            if (!AreCorrectDeletionParameters(parameters))
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            this.Service.DeleteRecords(parameters);
        }
    }
}
