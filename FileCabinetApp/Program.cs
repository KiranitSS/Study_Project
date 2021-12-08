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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Alexandr Alexeevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string ValidatorSettingsFile = "validation-rules.json";
        private static IFileCabinetService fileCabinetService;

        /// <summary>
        /// Gets or sets a value indicating whether stop app.
        /// </summary>
        /// <value>Is app running or stopped.</value>
        public static bool IsRunning { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether actions written in log.
        /// </summary>
        /// <value>Is app writting its actions is log.</value>
        public static bool IsLogging { get; set; }

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
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");

            IsLoggingStarted(args);

            using (FileStream fileStream = new FileStream("cabinet-records.db", FileMode.Create))
            {
                fileCabinetService = SetStorage(args, fileStream);
            }

            var commandHandler = CreateCommandHandlers();

            Console.WriteLine(HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Trim().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(HintMessage);
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
            string firstname;
            string lastname;
            DateTime dateOfbirth;
            decimal moneyCount;
            short pin;
            char charProp;
            RecordParameters parameters;

            do
            {
                Console.Write("First name: ");
                firstname = ReadInput(StringConverter);

                Console.Write("Last name: ");
                lastname = ReadInput(StringConverter);

                Console.Write("Date of bitrth: ");
                dateOfbirth = ReadInput(DateTimeConverter);

                Console.Write("Count of money: ");
                moneyCount = ReadInput(DecimalConverter);

                Console.Write("PIN code: ");
                pin = ReadInput(ShortConverter);

                Console.Write("Char prop: ");
                charProp = ReadInput(CharConverter);

                parameters = new RecordParameters(firstname, lastname, dateOfbirth, moneyCount, pin, charProp);

                if (Validator.ValidateParameters(parameters))
                {
                    break;
                }

                Console.WriteLine("Record creation failed!\n");
            }
            while (true);

            return parameters;
        }

        private static IRecordValidator CreateDefault(this ValidatorBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return ImportValidator(builder, "default");
        }

        private static IRecordValidator CreateCustom(this ValidatorBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return ImportValidator(builder, "custom");
        }

        private static IRecordValidator ImportValidator(ValidatorBuilder builder, string validatorType)
        {
            string json = File.ReadAllText(ValidatorSettingsFile);
            JObject settingsObj = JObject.Parse(json);

            JToken settingsToken = settingsObj[validatorType];

            JToken firstnameToken = settingsToken["firstName"];
            int minFirtnameLength = Convert.ToInt32(firstnameToken["min"].ToString(), CultureInfo.InvariantCulture);
            int maxFirstnameLength = Convert.ToInt32(firstnameToken["max"].ToString(), CultureInfo.InvariantCulture);

            JToken lastnameToken = settingsToken["lastName"];
            int minLastnameLength = Convert.ToInt32(lastnameToken["min"].ToString(), CultureInfo.InvariantCulture);
            int maxLastnameLength = Convert.ToInt32(lastnameToken["max"].ToString(), CultureInfo.InvariantCulture);

            JToken dateOfBirthToken = settingsToken["dateOfBirth"];
            DateTime minDateOfBirthBorder = DateTime.Parse(dateOfBirthToken["from"].ToString(), CultureInfo.InvariantCulture);
            DateTime maxDateOfBirthBorder = DateTime.Parse(dateOfBirthToken["to"].ToString(), CultureInfo.InvariantCulture);

            JToken moneyCountToken = settingsToken["moneyCount"];
            int minMoneyCount = Convert.ToInt32(moneyCountToken["min"].ToString(), CultureInfo.InvariantCulture);

            JToken pinToken = settingsToken["pin"];
            int minPinLength = Convert.ToInt32(pinToken["minLength"].ToString(), CultureInfo.InvariantCulture);

            JToken charPropToken = settingsToken["charProp"];
            bool mustBeLetter = Convert.ToBoolean(charPropToken["mustBeLetter"].ToString(), CultureInfo.InvariantCulture);

            return builder
                   .ValidateFirstName(minFirtnameLength, maxFirstnameLength)
                   .ValidateLastName(minLastnameLength, maxLastnameLength)
                   .ValidateDateOfBirth(minDateOfBirthBorder, maxDateOfBirthBorder)
                   .ValidateMoneyCount(minMoneyCount)
                   .ValidatePin(minPinLength)
                   .ValidateCharProp(mustBeLetter)
                   .Create();
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var updateHandler = new UpdateCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var purgehandler = new PurgeCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler();

            helpHandler.SetNext(createHandler).SetNext(statHandler).SetNext(updateHandler).SetNext(findHandler)
                .SetNext(listHandler).SetNext(exportHandler).SetNext(importHandler).SetNext(insertHandler)
                .SetNext(deleteHandler).SetNext(purgehandler).SetNext(exitHandler);

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
                return new ValidatorBuilder().CreateDefault();
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
                    return new ValidatorBuilder().CreateCustom();
                }
            }

            if (parameters.Length > 1 && parameters[0].Equals(shortValidationModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                validationMode = parameters[1];

                if (string.Equals(validationMode, customValidationModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using custom validation rules.");
                    return new ValidatorBuilder().CreateCustom();
                }
            }

            Console.WriteLine("Using default validation rules.");
            return new ValidatorBuilder().CreateDefault();
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
                storageMode = parameters[1];
                storageMode = storageMode.Trim();

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

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter)
        {
            do
            {
                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                return conversionResult.Item3;
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

        private static void IsLoggingStarted(string[] parameters)
        {
            IsLogging = parameters.Any(p => p.Equals("use-logger", StringComparison.OrdinalIgnoreCase));
        }
    }
}