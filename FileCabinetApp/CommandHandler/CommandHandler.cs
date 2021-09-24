using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandler
{
    public class CommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private ICommandHandler nextHandler;

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "create new record", "The 'create' command create new record." },
            new string[] { "edit", "edit record", "The 'edit' command edit record by id." },
            new string[] { "stat", "prints the stat", "The 'stat' command prints the stat." },
            new string[] { "list", "prints the records", "The 'list' command prints records list." },
            new string[] { "find", "finds matching records", "The 'find' command prints found records." },
            new string[] { "export", "exports the records", "The 'export' command exports records to external file." },
            new string[] { "import", "imports the records", "The 'import' command imports records from external file." },
            new string[] { "remove", "removes the record", "The 'remove' command removes record by id." },
            new string[] { "purge", "purges the record", "The 'purge' command clear db from removed records." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        public AppCommandRequest Handle(ICommandHandler commandHandler)
        {
            throw new NotImplementedException();
        }

        public void SetNext(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private static void Create(string command)
        {
            AddRecord(Program.FileCabinetService);
        }

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

        private static void List(string command)
        {
            List<FileCabinetRecord> records;

            if (Program.FileCabinetService.GetType().Equals(typeof(FileCabinetFilesystemService)))
            {
                records = (Program.FileCabinetService as FileCabinetFilesystemService).GetExistingRecords();
            }
            else
            {
                records = (Program.FileCabinetService as FileCabinetMemoryService).GetRecords().ToList();
            }

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

        private static void PrintTargetRecords(System.Collections.ObjectModel.ReadOnlyCollection<FileCabinetRecord> targetRecords)
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

        private static void Export(string parameters)
        {
            if (Program.FileCabinetService.GetStat() == 0)
            {
                Console.WriteLine("There is no any records");
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
                FileCabinetServiceSnapshot snapshot = Program.FileCabinetService.MakeSnapshot();

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

        private static void Import(string parameters)
        {
            parameters = parameters.Trim();

            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Not enougth parameters");
                return;
            }

            string[] importParams = parameters.Split(" ");

            if (!IsAbleToImport(importParams))
            {
                return;
            }

            using (StreamReader reader = new StreamReader(importParams[1]))
            {
                if (importParams[0].Equals("csv"))
                {
                    FileCabinetServiceSnapshot snapshot = Program.FileCabinetService.MakeSnapshot();
                    snapshot.LoadFromCsv(reader, Program.Validator);
                    Program.FileCabinetService.Restore(snapshot);
                }

                if (importParams[0].Equals("xml"))
                {
                    FileCabinetServiceSnapshot snapshot = Program.FileCabinetService.MakeSnapshot();
                    snapshot.LoadFromXml(reader, Program.Validator);
                    Program.FileCabinetService.Restore(snapshot);
                }
            }
        }

        private static void Remove(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters) || !int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            Program.FileCabinetService.RemoveRecord(id);
        }

        private static void Purge(string parameters)
        {
            Program.FileCabinetService.PurgeRecords();
        }

        private static void Stat(string parameters)
        {
            int recordsCount = Program.FileCabinetService.GetStat();

            if (Program.FileCabinetService.GetType() == typeof(FileCabinetFilesystemService))
            {
                int removedRecordsCount = recordsCount - (Program.FileCabinetService as FileCabinetFilesystemService).GetExistingRecords().Count;
                int existingRecordsCount = (Program.FileCabinetService as FileCabinetFilesystemService).GetExistingRecords().Count;

                Console.WriteLine($"Here {existingRecordsCount} record(s).");
                Console.WriteLine($"And here {removedRecordsCount} removed records.");
                return;
            }

            Console.WriteLine($"Here {recordsCount} record(s).\nAnd here 0 removed records.");
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }

        private static void PrintRecord(FileCabinetRecord record)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, " +
                $"{record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{record.PIN}, {record.MoneyCount}$, {record.CharProp}");
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

        private static bool IsAbleToImport(string[] importParams)
        {
            if (importParams.Length != 2)
            {
                Console.WriteLine("Incorrect input parameters");
                return false;
            }

            if (!File.Exists(importParams[1]))
            {
                Console.WriteLine($"Import error: file {importParams[1]} is not exist.");
                return false;
            }

            return true;
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

        private static string GetFileName(string filePath)
        {
            if (filePath.Contains("\\"))
            {
                return filePath[(filePath.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1) ..];
            }

            return filePath;
        }

        private static string GetTargetName(string parameters, int targetPropLength)
        {
            int startIndex = targetPropLength;
            return parameters[(startIndex + 1) ..];
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

        private static ReadOnlyCollection<FileCabinetRecord> FindTargetRecords(string targetValue, string targetProp)
        {
            if (string.Equals(targetProp, "firstname", StringComparison.OrdinalIgnoreCase))
            {
                return Program.FileCabinetService.FindByFirstName(targetValue);
            }

            if (string.Equals(targetProp, "lastname", StringComparison.OrdinalIgnoreCase))
            {
                return Program.FileCabinetService.FindByLastName(targetValue);
            }

            if (string.Equals(targetProp, "dateofbirth", StringComparison.OrdinalIgnoreCase))
            {
                return Program.FileCabinetService.FindByBirthDate(targetValue);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
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
            bool result = Program.Validator.IsCorrectFirstName(value);
            return new (result, value);
        }

        private static Tuple<bool, string> LastnameValidator(string value)
        {
            bool result = Program.Validator.IsCorrectLastName(value);
            return new (result, value);
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime value)
        {
            bool result = Program.Validator.IsCorrectDateOfBirth(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> MoneyCountValidator(decimal value)
        {
            bool result = Program.Validator.IsCorrectMoneyCount(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> PINValidator(short value)
        {
            bool result = Program.Validator.IsCorrectPIN(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> CharPropValidator(char value)
        {
            bool result = Program.Validator.IsCorrectCharProp(value);
            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }

        private static Tuple<bool, string> IDValidator(int value)
        {
            bool result = false;

            if (value < Program.FileCabinetService.GetStat() + 1 && value > 0)
            {
                result = true;
            }

            return new (result, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
