using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : CommandHandlerBase
    {
        private static void Edit(string command)
        {
            Console.Write("Write ID:");
            int id = ReadInput(IDConverter, IDValidator);

            if (id > Program.FileCabinetService.GetStat())
            {
                Console.WriteLine("Incorrect ID");
            }

            var parameters = GetRecordData();
            Program.FileCabinetService.EditRecord(id, parameters);
        }
    }
}
