using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler for showing apps methods.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "create", "create new record", "The 'create' command create new record." },
            new string[] { "edit", "edit record", "The 'edit' command edit record by id." },
            new string[] { "stat", "prints the stat", "The 'stat' command prints the stat." },
            new string[] { "list", "prints the records", "The 'list' command prints records list." },
            new string[] { "find", "finds matching records", "The 'find' command prints found records." },
            new string[] { "export", "exports the records", "The 'export' command exports records to external file." },
            new string[] { "import", "imports the records", "The 'import' command imports records from external file." },
            new string[] { "remove", "removes the record", "The 'remove' command removes record by id." },
            new string[] { "delete", "deletes the records", "The 'delete' command deletes records by condition." },
            new string[] { "purge", "purges the record", "The 'purge' command clear db from removed records." },
            new string[] { "insert", "insert the record", "The 'insert' command add record with parameters." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                PrintHelp(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
