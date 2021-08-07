using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

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
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "create new record", "The 'create' command create new record." },
            new string[] { "edit", "edit record", "The 'edit' command edit record by id." },
            new string[] { "stat", "prints the stat", "The 'stat' command prints the stat." },
            new string[] { "list", "prints the records", "The 'list' command prints records list." },
            new string[] { "find", "finds matching records", "The 'find' command prints found records." },
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

        private static void Create(string command)
        {
            RecordsUtils.AddRecord(fileCabinetService);
        }

        private static void Edit(string command)
        {
            int id = GetId();

            fileCabinetService.EditRecord(id);
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
                PrintRecord(record);
            }
        }

        private static void Find(string parameters)
        {
            string targetProp = parameters.Trim();

            if (string.IsNullOrEmpty(targetProp))
            {
                Console.WriteLine("Property name missed");
                return;
            }

            int startIndex = targetProp.IndexOf(" ", StringComparison.InvariantCulture);

            if (startIndex == -1)
            {
                Console.WriteLine("Property value missed");
                return;
            }

            targetProp = targetProp.Substring(0, startIndex);

            if (parameters.Length == targetProp.Length)
            {
                Console.WriteLine("Property value missed");
                return;
            }

            string targetName = GetTargetName(parameters, targetProp);

            var targetRecords = FindTargetRecords(targetName, targetProp);

            PrintTargetRecords(targetRecords);
        }

        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat();
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

        private static int GetId()
        {
            Console.Write("\nRecord ID: ");

            int id = -1;
            bool isCorrect = false;

            while (!isCorrect)
            {
                while (!int.TryParse(Console.ReadLine(), out id))
                {
                    Console.Write("\nWrite correct ID: ");
                }

                if (id > 0 && id < fileCabinetService.GetStat() + 1)
                {
                    isCorrect = true;
                    continue;
                }

                Console.WriteLine("Record is not found.");
                Console.Write("\nWrite correct ID: ");
            }

            return id;
        }

        private static void PrintRecord(FileCabinetRecord record)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, " +
                $"{record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{record.ShortProp}, {record.MoneyCount}$, {record.CharProp}");
        }

        private static string GetTargetName(string parameters, string targetProp)
        {
            return parameters.Substring(
                targetProp.Length + 1,
                parameters.Length - (targetProp.Length + 1));
        }

        private static FileCabinetRecord[] FindTargetRecords(string targetValue, string targetProp)
        {
            if (string.Equals(targetProp, "firstname", StringComparison.OrdinalIgnoreCase))
            {
                return fileCabinetService.FindByFirstName(targetValue);
            }

            if (string.Equals(targetProp, "lastname", StringComparison.OrdinalIgnoreCase))
            {
                return fileCabinetService.FindByLastName(targetValue);
            }

            if (string.Equals(targetProp, "dateofbirth", StringComparison.OrdinalIgnoreCase))
            {
                return fileCabinetService.FindByBirthDate(targetValue);
            }

            return Array.Empty<FileCabinetRecord>();
        }

        private static void PrintTargetRecords(FileCabinetRecord[] targetRecords)
        {
            if (targetRecords.Length == 0)
            {
                Console.WriteLine("There are no suitable entries");
            }
            else
            {
                foreach (var record in targetRecords)
                {
                    PrintRecord(record);
                }
            }
        }
    }
}