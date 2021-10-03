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
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Alexandr Alexeevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static IFileCabinetService fileCabinetService;

        /// <summary>
        /// Gets or sets a value indicating whether stop app.
        /// </summary>
        /// <value>Is app running or stopped.</value>
        public static bool IsRunning { get; set; } = true;

        /// <summary>
        /// Gets or sets validation setting.
        /// </summary>
        /// <value>
        /// Validation setting.
        /// </value>
        public static IRecordValidator Validator { get; set; }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Program start parameters.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            using (FileStream fileStream = new FileStream("cabinet-records.db", FileMode.Create))
            {
                fileCabinetService = SetStorage(args, fileStream);
            }

            var commandHandler = CreateCommandHandlers();

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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                commandHandler.Handle(new AppCommandRequest { Command = command, Parameters = parameters });
            }
            while (IsRunning);
        }

        /// <summary>
        /// Gets records parameters from console input.
        /// </summary>
        /// <returns>Values for creation record.</returns>
        public static RecordParameters GetRecordData()
        {
            Console.Write("First name: ");
            string firstname = ReadInput(StringConverter, FirstnameValidator);

            Console.Write("Last name: ");
            string lastname = ReadInput(StringConverter, LastnameValidator);

            Console.Write("Date of bitrth: ");
            DateTime dateOfbirth = ReadInput(DateTimeConverter, DateOfBirthValidator);

            Console.Write("Count of money: ");
            decimal moneyCount = ReadInput(DecimalConverter, MoneyCountValidator);

            Console.Write("PIN code: ");
            short pin = ReadInput(ShortConverter, PINValidator);

            Console.Write("Char prop: ");
            char charProp = ReadInput(CharConverter, CharPropValidator);

            return new RecordParameters(firstname, lastname, dateOfbirth, moneyCount, pin, charProp);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgehandler = new PurgeCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler();

            helpHandler.SetNext(createHandler).SetNext(statHandler).SetNext(editHandler).SetNext(findHandler)
                .SetNext(listHandler).SetNext(exportHandler).SetNext(importHandler).SetNext(removeHandler)
                .SetNext(purgehandler).SetNext(exitHandler);

            return helpHandler;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            records.ToList().ForEach(rec => Console.WriteLine($"#{rec.Id}, {rec.FirstName}, " +
                $"{rec.LastName}, {rec.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{rec.PIN}, {rec.MoneyCount}$, {rec.CharProp}"));
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

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string, string> StringConverter(string value)
        {
            return new (true, value, value);
        }

        private static Tuple<bool, string, DateTime> DateTimeConverter(string value)
        {
            bool result = DateTime.TryParse(value, out DateTime conversionResult);
            return new (result, value, conversionResult);
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string value)
        {
            bool result = decimal.TryParse(value, out decimal conversionResult);
            return new (result, value, conversionResult);
        }

        private static Tuple<bool, string, short> ShortConverter(string value)
        {
            bool result = short.TryParse(value, out short conversionResult);
            return new (result, value, conversionResult);
        }

        private static Tuple<bool, string, char> CharConverter(string value)
        {
            bool result = char.TryParse(value, out char conversionResult);
            return new (result, value, conversionResult);
        }

        private static Tuple<bool, string> FirstnameValidator(string value)
        {
            bool result = Program.Validator.ValidateFirstName(value);
            return new (result, value);
        }

        private static Tuple<bool, string> LastnameValidator(string value)
        {
            bool result = Program.Validator.ValidateLastName(value);
            return new (result, value);
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime value)
        {
            bool result = Program.Validator.ValidateDateOfBirth(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> MoneyCountValidator(decimal value)
        {
            bool result = Program.Validator.ValidateMoneyCount(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> PINValidator(short value)
        {
            bool result = Program.Validator.ValidatePIN(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> CharPropValidator(char value)
        {
            bool result = Program.Validator.ValidateProp(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}