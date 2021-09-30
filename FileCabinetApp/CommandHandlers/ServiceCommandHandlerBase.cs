using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.Service = service;
        }

        /// <summary>
        /// Gets or sets service for handling records.
        /// </summary>
        /// <value>
        /// Service for handling records.
        /// </value>
        protected IFileCabinetService Service { get; set; }
    }
}
