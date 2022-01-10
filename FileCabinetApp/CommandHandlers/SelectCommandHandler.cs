using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public SelectCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("select", StringComparison.OrdinalIgnoreCase))
            {
                this.Select(request.Parameters);
            }

            return base.Handle(request);
        }

        private void Select(string parameters)
        {
            this.Service.SelectRecords(parameters);
        }
    }
}
