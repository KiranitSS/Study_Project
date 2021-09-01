using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Alexandr Alexeevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator validator;
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
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
            new string[] { "export", "exports the records", "The 'export' command exports records to external file." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Program start parameters.</param>
        public static void Main(string[] args)
        {
            args = new string[] { "-s", "file" };

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            using (FileStream fileStream = new FileStream("cabinet-records.db", FileMode.Append))
            {
                fileCabinetService = SetStorage(args, fileStream);
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
            while (isRunning);
        }

        private static IRecordValidator GetValidator(string[] mainParams)
        {
            if (mainParams is null || mainParams.Length == 0)
            {
                Console.WriteLine("Using default validation rules.");
                return new DefaultValidator();
            }

            string validationMode;
            string validationModeMessage = "-validation-rules=";
            string shortValidationModeMessage = "-v";
            string customValidationModeText = "custom";

            if (mainParams.Length > 0 && mainParams[0].Contains(validationModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                validationMode = mainParams[0];
                validationMode = validationMode.Trim();

                validationMode = validationMode.Replace(validationModeMessage, string.Empty, StringComparison.OrdinalIgnoreCase);

                if (validationMode.Equals(customValidationModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using custom validation rules.");
                    return new CustomValidator();
                }
            }

            if (mainParams.Length > 1 && mainParams[0].Equals(shortValidationModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                validationMode = mainParams[1];

                if (string.Equals(validationMode, customValidationModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using custom validation rules.");
                    return new CustomValidator();
                }
            }

            Console.WriteLine("Using default validation rules.");
            return new DefaultValidator();
        }

        private static IFileCabinetService SetStorage(string[] mainParams, FileStream fileStream)
        {
            if (mainParams is null || mainParams.Length == 0)
            {
                Console.WriteLine("Using memory storage.");
                validator = GetValidator(mainParams);
                return new FileCabinetMemoryService(validator);
            }

            string storageMode;
            string storageModeMessage = "--storage";
            string shortStorageModeMessage = "-s";
            string customStorageModeText = "file";

            if (mainParams.Length > 0 && mainParams[0].Contains(storageModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                storageMode = mainParams[0];
                storageMode = storageMode.Trim();

                storageMode = storageMode.Replace(storageModeMessage, string.Empty, StringComparison.OrdinalIgnoreCase);

                if (storageMode.Equals(customStorageModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using system storage.");
                    validator = GetValidator(mainParams);
                    return new FileCabinetFilesystemService(fileStream);
                }
            }

            if (mainParams.Length > 1 && mainParams[0].Equals(shortStorageModeMessage, StringComparison.OrdinalIgnoreCase))
            {
                storageMode = mainParams[1];

                if (string.Equals(storageMode, customStorageModeText, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Using system storage.");
                    validator = GetValidator(mainParams);
                    return new FileCabinetFilesystemService(fileStream);
                }
            }

            Console.WriteLine("Using memory storage.");
            validator = GetValidator(mainParams);
            return new FileCabinetMemoryService(validator);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void Create(string command)
        {
            AddRecord(fileCabinetService);
        }

        private static void Edit(string command)
        {
            Console.Write("Write ID:");
            int id = ReadInput(IDConverter, IDValidator);

            if (id > fileCabinetService.GetStat())
            {
                Console.WriteLine("Incorrect ID");
            }

            var parameters = GetRecordData();
            fileCabinetService.EditRecord(id, parameters);
        }

        private static RecordParameters GetRecordData()
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

        private static void List(string command)
        {
            var records = fileCabinetService.GetRecords();

            if (records.Count == 0)
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
            string targetProp = GetTargetProp(parameters);

            if (parameters.Length == targetProp.Length)
            {
                Console.WriteLine("Property value missed");
                return;
            }

            string targetName = GetTargetName(parameters, targetProp.Length);

            var targetRecords = FindTargetRecords(targetName, targetProp);

            PrintTargetRecords(targetRecords);
        }

        private static void Export(string parameters)
        {
            if (fileCabinetService.GetStat() == 0)
            {
                Console.WriteLine("There is no any records.");
                return;
            }

            string fileType = GetTargetProp(parameters);

            if (IsFileNameMissed(parameters, fileType))
            {
                return;
            }

            string filePath = parameters[fileType.Length..].Trim();
            string fileName = GetFileName(filePath);

            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Operation failed, empty path.");
                return;
            }

            filePath = filePath.Remove(filePath.Length - fileName.Length);

            if (!fileName.Contains($".{fileType}"))
            {
                fileName += $".{fileType}";
            }

            filePath += fileName;

            if (!IsAbleToSave(filePath, fileName))
            {
                return;
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                FileCabinetServiceSnapshot snapshot = ((FileCabinetMemoryService)fileCabinetService).MakeSnapshot();

                if (fileType.Equals("csv", StringComparison.OrdinalIgnoreCase))
                {
                    snapshot.SaveToCsv(writer);
                }

                if (fileType.Equals("xml", StringComparison.OrdinalIgnoreCase))
                {
                    snapshot.SaveToXml(writer);
                }
            }
        }

        private static bool IsAbleToSave(string filePath, string fileName)
        {
            if (!IsDirectoryValid(filePath, fileName))
            {
                return false;
            }

            if (File.Exists(filePath))
            {
                Console.Write($"File is exist - rewrite {filePath}? [Y/n]");
                string answer;

                do
                {
                    answer = Console.ReadLine();

                    if (answer.Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    if (answer.Equals("n", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Operation canceled");
                        return false;
                    }

                    Console.Write("Incorrect input, retry:");
                }
                while (true);
            }
            else
            {
                return true;
            }
        }

        private static bool IsDirectoryValid(string filePath, string fileName)
        {
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                Console.WriteLine($"Export failed: can't open file {fileName}.");
                return false;
            }

            filePath = filePath.Remove(filePath.Length - fileName.Length);

            if (!Directory.Exists(filePath) && filePath.Contains("\\"))
            {
                Console.WriteLine($"Export failed: can't open file {fileName}.");
                return false;
            }

            return true;
        }

        private static bool IsFileNameMissed(string parameters, string fileType)
        {
            if (parameters.Length != fileType.Length)
            {
                return false;
            }
            else
            {
                Console.WriteLine("Operation failed.");
                return true;
            }
        }

        private static string GetTargetProp(string parameters)
        {
            string targetProp = parameters.Trim();

            if (string.IsNullOrEmpty(targetProp))
            {
                Console.WriteLine("Property name missed");
                return targetProp;
            }

            int endIndex = targetProp.IndexOf(" ", StringComparison.InvariantCulture);

            if (endIndex == -1)
            {
                Console.WriteLine("Property value missed");
                return targetProp;
            }

            return targetProp[..endIndex];
        }

        private static string GetFileName(string filePath)
        {
            if (filePath.Contains("\\"))
            {
                return filePath[(filePath.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1) ..];
            }

            return filePath;
        }

        private static void AddRecord(IFileCabinetService fileCabinetService)
        {
            if (fileCabinetService is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetService));
            }

            var parameters = GetRecordData();
            int recId = fileCabinetService.CreateRecord(parameters);

            if (recId == -1)
            {
                Console.WriteLine($"Record is not created.");
            }
            else
            {
                Console.WriteLine($"Record #{recId} is created.");
            }
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

        private static void PrintRecord(FileCabinetRecord record)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, " +
                $"{record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{record.PIN}, {record.MoneyCount}$, {record.CharProp}");
        }

        private static string GetTargetName(string parameters, int targetPropLength)
        {
            int startIndex = targetPropLength;
            return parameters[(startIndex + 1) ..];
        }

        private static ReadOnlyCollection<FileCabinetRecord> FindTargetRecords(string targetValue, string targetProp)
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

            return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
        }

        private static void PrintTargetRecords(ReadOnlyCollection<FileCabinetRecord> targetRecords)
        {
            if (targetRecords.Count == 0)
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

        private static Tuple<bool, string, int> IDConverter(string value)
        {
            bool result = int.TryParse(value, out int conversionResult);
            return new (result, value, conversionResult);
        }

        private static Tuple<bool, string> FirstnameValidator(string value)
        {
            bool result = validator.IsCorrectFirstName(value);
            return new (result, value);
        }

        private static Tuple<bool, string> LastnameValidator(string value)
        {
            bool result = validator.IsCorrectLastName(value);
            return new (result, value);
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime value)
        {
            bool result = validator.IsCorrectDateOfBirth(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> MoneyCountValidator(decimal value)
        {
            bool result = validator.IsCorrectMoneyCount(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> PINValidator(short value)
        {
            bool result = validator.IsCorrectPIN(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> CharPropValidator(char value)
        {
            bool result = validator.IsCorrectCharProp(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> IDValidator(int value)
        {
            bool result = false;

            if (value < fileCabinetService.GetStat() + 1 && value > 0)
            {
                result = true;
            }

            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}