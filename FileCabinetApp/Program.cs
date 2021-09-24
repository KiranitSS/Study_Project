using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Alexandr Alexeevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        public static bool IsRunning { get; set; } = true;

        public static IRecordValidator Validator { get; set; }

        public static IFileCabinetService FileCabinetService { get; set; }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Program start parameters.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            using (FileStream fileStream = new FileStream("cabinet-records.db", FileMode.Create))
            {
                FileCabinetService = SetStorage(args, fileStream);
            }

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
            while (IsRunning);
        }

        private static IRecordValidator GetValidator(string[] parameters)
        {
            if (parameters is null || parameters.Length == 0)
            {
                Console.WriteLine("Using default validation rules.");
                return new DefaultValidator();
            }

            string validationMode;
            string validationModeMessage = "-validation-rules=";
            string shortValidationModeMessage = "-v";
            string customValidationModeText = "custom";

            if (parameters.Length > 0 && parameters[0].Contains(validationModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                validationMode = parameters[0];
                validationMode = validationMode.Trim();

                validationMode = validationMode.Replace(validationModeMessage, string.Empty, StringComparison.OrdinalIgnoreCase);

                if (validationMode.Equals(customValidationModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using custom validation rules.");
                    return new CustomValidator();
                }
            }

            if (parameters.Length > 1 && parameters[0].Equals(shortValidationModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                validationMode = parameters[1];

                if (string.Equals(validationMode, customValidationModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using custom validation rules.");
                    return new CustomValidator();
                }
            }

            Console.WriteLine("Using default validation rules.");
            return new DefaultValidator();
        }

        private static IFileCabinetService SetStorage(string[] parameters, FileStream fileStream)
        {
            if (parameters is null || parameters.Length == 0)
            {
                Console.WriteLine("Using memory storage.");
                Validator = GetValidator(parameters);
                return new FileCabinetMemoryService(Validator);
            }

            string storageMode;
            string storageModeMessage = "--storage";
            string shortStorageModeMessage = "-s";
            string customStorageModeText = "file";

            if (parameters.Length > 0 && parameters[0].Contains(storageModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                storageMode = parameters[0];
                storageMode = storageMode.Trim();

                storageMode = storageMode.Replace(storageModeMessage, string.Empty, StringComparison.OrdinalIgnoreCase);

                if (storageMode.Equals(customStorageModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using system storage.");
                    Validator = GetValidator(parameters);
                    return new FileCabinetFilesystemService(fileStream);
                }
            }

            if (parameters.Length > 1 && parameters[0].Equals(shortStorageModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                storageMode = parameters[1];

                if (string.Equals(storageMode, customStorageModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using system storage.");
                    Validator = GetValidator(parameters);
                    return new FileCabinetFilesystemService(fileStream);
                }
            }

            Console.WriteLine("Using memory storage.");
            Validator = GetValidator(parameters);
            return new FileCabinetMemoryService(Validator);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}