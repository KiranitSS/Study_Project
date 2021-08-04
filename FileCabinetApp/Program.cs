using System;
using System.ComponentModel;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Alexandr Alexeevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "create new record", "The 'create' command create new record." },
            new string[] { "stat", "prints the stat", "The 'stat' command prints the stat." },
            new string[] { "list", "prints the records", "The 'list' command prints records list." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("\nLast name: ");
            string lastName = Console.ReadLine();

            Console.Write("\nDate of birth: ");
            DateTime birth;

            while (!DateTime.TryParse(Console.ReadLine(), out birth))
            {
                Console.Write("\nWrite correct date (date format month/day/year): ");
            }

            Console.Write("\nMoney count: ");
            decimal moneyCount;

            while (!decimal.TryParse(Console.ReadLine(), out moneyCount))
            {
                Console.Write("\nWrite correct money count (must contains only digits and be bigger than 200): ");
            }

            Console.WriteLine("\nShort numbers: ");
            short anyShort;

            while (!short.TryParse(Console.ReadLine(), out anyShort))
            {
                Console.Write("\nWrite correct short number (digits count from 1 to 5): ");
            }

            Console.Write("\nWrite any letter: ");
            char anyChar;

            while (!char.TryParse(Console.ReadLine(), out anyChar))
            {
                Console.Write("\nWrite correct symbol (One symbol!): ");
            }

            int recId = fileCabinetService.CreateRecord(firstName, lastName, birth, anyShort, moneyCount, anyChar);

            Console.WriteLine($"Record #{recId} is created.");
        }

        private static void List(string command)
        {
            var records = fileCabinetService.GetRecords();

            if (records.Length == 0)
            {
                Console.WriteLine("There are no any records");
                return;
            }

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, " +
                    $"{record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                    $"{record.ShortProp}, {record.MoneyCount}, ${record.CharProp}");
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"Here {recordsCount} record(s).");
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}