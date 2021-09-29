using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents class for creating requests and store their parameters.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets or sets a command for the launch handlers.
        /// </summary>
        /// <value>
        /// A command for the launch handlers.
        /// </value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets parameters for command.
        /// </summary>
        /// <value>
        /// Parameters for command.
        /// </value>
        public string Parameters { get; set; }
    }
}
