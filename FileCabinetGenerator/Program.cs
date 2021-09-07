using System;
using System.Linq;
using System.Collections.Generic;
using FileCabinetApp;
using System.IO;
using System.Xml;

namespace FileCabinetGenerator
{
    static class Program
    {
        private static string path;
        private static FileCabinetMemoryService fileCabinetService;
        private static List<string> cmdParams;
        private static int recordsAmount;
        private static int startGeneratingId;
        static void Main(string[] args)
        {
            if (args is null)
            {
                Console.WriteLine($"{nameof(args)} is null");
                return;
            }

            if (!TrySetOutput(args))
            {
                return;
            }

            Console.ReadLine();
        }

        private static bool TrySetOutput(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Not enougth parameters.");
                return false;
            }

            cmdParams = new List<string>(args);

            string outputTypeMsg = "--output-type=";
            string shortOutputTypeMsg = "-t";
            string outputPathMsg = "--output=";
            string shortOutputPathMsg = "-o";
            string recordsAmountMsg = "--records-amount=";
            string shortRecordsAmountMsg = "-a";
            string startIdMsg = "--start-id=";
            string shortStartIdMsg = "-i";


            if (!TrySetSetting(outputTypeMsg, shortOutputTypeMsg, TrySetOutputType))
            {
                return false;
            }

            if (!TrySetSetting(outputPathMsg, shortOutputPathMsg, TrySetOutputPath))
            {
                return false;
            }

            if (!TrySetSetting(recordsAmountMsg, shortRecordsAmountMsg, TrySetRecordsAmount))
            {
                return false;
            }

            if (!TrySetSetting(startIdMsg, shortStartIdMsg, TrySetStartId))
            {
                return false;
            }

            return true;
        }

        private static bool TrySetSetting(string longMsg, string shortMsg, Func<string, bool> settingsSetter)
        {
            if (cmdParams.Count >= 1 && cmdParams[0].Length > longMsg.Length && cmdParams[0][..longMsg.Length].Equals(longMsg))
            {
                if (settingsSetter(cmdParams[0].Remove(0, longMsg.Length)))
                {
                    cmdParams.Remove(cmdParams[0]);
                    return true;
                }

                return false;

            }

            if (cmdParams.Count < 2)
            {
                return false;
            }

            if (cmdParams[0].Equals(shortMsg) && settingsSetter(cmdParams[1]))
            {
                cmdParams.Remove(cmdParams[0]);
                cmdParams.Remove(cmdParams[0]);

                return true;
            }

            return false;
        }

        private static bool TrySetOutputType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return false;
            }

            if (type.Equals("csv"))
            {
                fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                Console.WriteLine("CSV");
                return true;
            }

            if (type.Equals("xml"))
            {
                fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                Console.WriteLine("XML");
                return true;
            }

            return false;
        }

        private static bool TrySetOutputPath(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1)
            {
                if (filePath.Contains("\\"))
                {
                    if (!Directory.Exists(filePath[..filePath.LastIndexOf("\\")]))
                    {
                        Directory.CreateDirectory(filePath[..filePath.LastIndexOf("\\")]);
                    }

                    path = filePath;
                    return true;

                }
                else
                {
                    path = filePath;
                    return true;

                }
            }

            return false;
        }

        private static bool TrySetRecordsAmount(string amount)
        {
            if (string.IsNullOrWhiteSpace(amount))
            {
                return false;
            }

            if (int.TryParse(amount, out recordsAmount))
            {
                return true;
            }

            return false;
        }

        private static bool TrySetStartId(string startId)
        {
            if (string.IsNullOrWhiteSpace(startId))
            {
                return false;
            }

            if (int.TryParse(startId, out startGeneratingId))
            {
                return true;
            }

            return false;
        }
    }
}
