using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
     /// Represents handler class for correction users commands.
     /// </summary>
    public class CorrectionCommandHandler : CommandHandlerBase
    {
        private static readonly string[] Commands = new string[]
        {
            "list",
            "stat",
            "create",
            "delete",
            "update",
            "purge",
            "find",
            "help",
            "import",
            "export",
            "exit",
            "insert",
        };

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null)
            {
                ShowSameCommands(request.Command);
            }

            return base.Handle(request);
        }

        private static void ShowSameCommands(string inputCommand)
        {
            foreach (var command in Commands)
            {
                if (IsSameCommand(inputCommand, command))
                {
                    Console.WriteLine($"The same command is {command}");
                }
            }
        }

        private static bool IsSameCommand(string inputCommand, string command)
        {
            if (string.IsNullOrWhiteSpace(inputCommand))
            {
                Console.WriteLine("Command can't be empty.");
                return false;
            }

            int diff = command.Except(inputCommand).ToList().Count;
            int maxDiff = 2;

            if (diff == 0 && Math.Abs(command.Length - inputCommand.Length) < 2)
            {
                return true;
            }

            if (diff > maxDiff || Math.Abs(command.Length - inputCommand.Length) > 2)
            {
                return false;
            }

            if (inputCommand[0] != command[0])
            {
                return false;
            }

            int diffCounter = GetWordsDiff(inputCommand, command);

            if (diffCounter > maxDiff)
            {
                return false;
            }

            return true;
        }

        private static int GetWordsDiff(string inputCommand, string command)
        {
            int counter = 0;
            int diffCounter = 0;

            StringBuilder builder = new StringBuilder(inputCommand);

            foreach (var letter in command)
            {
                if (builder.Length <= counter)
                {
                    break;
                }

                if (builder[counter] != letter)
                {
                    diffCounter++;
                }

                counter++;
            }

            return diffCounter;
        }
    }
}
