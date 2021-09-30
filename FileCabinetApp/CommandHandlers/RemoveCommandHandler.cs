using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for removing record.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public RemoveCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("remove", StringComparison.OrdinalIgnoreCase))
            {
                this.Remove(request.Parameters);
            }

            return base.Handle(request);
        }

        private void Remove(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters) || !int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            this.Service.RemoveRecord(id);
        }
    }
}
