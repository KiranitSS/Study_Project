using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class CorrectionCommandHandler : CommandHandlerBase
    {
        private static readonly string[] Commands = { "list", "stat", "create", "delete", "update", "find", "import", "export", "insert", "purge", "help", "exit" };

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null)
            {
                CheckCommandParameters(request.Command);
            }

            return base.Handle(request);
        }

        private static void CheckCommandParameters(string parameters)
        {
            foreach (var command in Commands)
            {
                if (IsSameToCommand(parameters, command))
                {
                    Console.WriteLine($"Is same to {command}");
                }
            }
        }

        private static bool IsSameToCommand(string inputCommand, string command)
        {
            if (string.IsNullOrWhiteSpace(inputCommand))
            {
                Console.WriteLine("Command can't be empty.");
                return false;
            }

            int counter = 0;
            int minBorder = command.Length / 2;

            if (inputCommand.Length > command.Length + 2 || inputCommand.Length < command.Length - 2)
            {
                return false;
            }

            if (inputCommand[0] != command[0])
            {
                return false;
            }

            foreach (var letter in inputCommand)
            {
                if (command.Contains(letter))
                {
                    command = command.Remove(command.IndexOf(letter), 1);
                    counter++;
                }
            }

            if (counter < minBorder)
            {
                return false;
            }

            return true;
        }
    }
}
