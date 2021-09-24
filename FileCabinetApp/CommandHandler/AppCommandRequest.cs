using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandler
{
    public class AppCommandRequest
    {
        public string Command { get; set; }

        public string Parameters { get; set; }
    }
}
