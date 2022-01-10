using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for inserting records.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public InsertCommandHandler(IFileCabinetService service)
    : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("insert", StringComparison.OrdinalIgnoreCase))
            {
                this.Insert(request.Parameters);
            }

            return base.Handle(request);
        }

        private static bool IsAbleToInsert(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!parameters.Contains("values", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static bool TryGetInputData(string parameters, out Dictionary<string, string> inputData)
        {
            var data = parameters.Replace(")", string.Empty)
                .Replace("(", string.Empty)
                .Replace(" ", string.Empty)
                .Split("values");

            string[] keys = data[0].Split(",");
            string[] values = data[1].Replace("'", string.Empty).Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (keys.Length != values.Length || keys.Length != typeof(FileCabinetRecord).GetProperties().Length)
            {
                inputData = new Dictionary<string, string>();
                Console.WriteLine("Some values missed.");
                return false;
            }

            inputData = new Dictionary<string, string>();

            for (int i = 0; i < keys.Length; i++)
            {
                inputData.Add(keys[i], values[i]);
            }

            return true;
        }

        private static bool CheckInputData(Dictionary<string, string> inputData)
        {
            if (!inputData.TryGetValue("id", out _) || !int.TryParse(inputData["id"], out _))
            {
                return false;
            }

            if (!inputData.TryGetValue("moneycount", out _) || !decimal.TryParse(inputData["moneycount"], out _))
            {
                return false;
            }

            if (!inputData.TryGetValue("pin", out _) || !short.TryParse(inputData["pin"], out _))
            {
                return false;
            }

            if (!inputData.TryGetValue("charprop", out _) || !char.TryParse(inputData["charprop"], out _))
            {
                return false;
            }

            return true;
        }

        private void Insert(string parameters)
        {
            if (!IsAbleToInsert(parameters))
            {
                Console.WriteLine("Values missed");
                return;
            }

            if (!TryGetInputData(parameters, out Dictionary<string, string> inputData))
            {
                Console.WriteLine("Incorrect input");
                return;
            }

            if (!CheckInputData(inputData))
            {
                Console.WriteLine("Incorrect input");
                return;
            }

            var data = new RecordParameters
            {
                Id = int.Parse(inputData["id"], CultureInfo.InvariantCulture),
                FirstName = inputData["firstname"],
                LastName = inputData["lastname"],
                DateOfBirth = DateTime.Parse(inputData["dateofbirth"], CultureInfo.InvariantCulture),
                MoneyCount = decimal.Parse(inputData["moneycount"], CultureInfo.InvariantCulture),
                PIN = short.Parse(inputData["pin"], CultureInfo.InvariantCulture),
                CharProp = char.Parse(inputData["charprop"]),
            };

            if (!Program.Validator.ValidateParameters(data))
            {
                Console.WriteLine("Incorrect input");
                return;
            }

            this.Service.InsertRecord(data);
            Console.WriteLine("Record inserted");
        }
    }
}
