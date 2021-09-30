using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler for exporting records.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public ExportCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("export", StringComparison.OrdinalIgnoreCase))
            {
                this.Export(request.Parameters);
            }

            return base.Handle(request);
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

        private void Export(string parameters)
        {
            if (this.service.GetStat() == 0)
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
                FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();

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
    }
}
