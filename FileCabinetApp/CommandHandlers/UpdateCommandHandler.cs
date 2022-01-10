using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents handler class for updating records data.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service for working with records.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request != null && request.Command.Equals("update", StringComparison.OrdinalIgnoreCase))
            {
                this.Update(request.Parameters);
            }

            return base.Handle(request);
        }

        private static bool AreCorrectUpdateParameters(string parameters)
        {
            if (parameters.IndexOf("set", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            parameters = parameters.Replace("set", string.Empty).TrimStart();

            return parameters.Contains('=') && parameters.Contains(" where ", StringComparison.OrdinalIgnoreCase);
        }

        private static Dictionary<string, string> GetDataForUpdate(string parameters)
        {
            Dictionary<string, string> dataForUpdate = new Dictionary<string, string>();
            var updateParameters = parameters.Replace("'", string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries);
            int separatorIndex;

            foreach (var parameter in updateParameters)
            {
                separatorIndex = parameter.IndexOf("=", StringComparison.OrdinalIgnoreCase);

                if (separatorIndex == -1)
                {
                    return new Dictionary<string, string>();
                }

                dataForUpdate.Add(parameter[..separatorIndex].ToUpperInvariant(), parameter[(separatorIndex + 1) ..]);
            }

            if (dataForUpdate.ContainsKey("ID"))
            {
                dataForUpdate.Remove("ID");
                Console.WriteLine("The id parameter is immutable");
            }

            return dataForUpdate;
        }

        private void Update(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Not enougth parameters");
                return;
            }

            parameters = parameters.Trim();

            if (!AreCorrectUpdateParameters(parameters))
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            var inputs = parameters.Remove(0, 3).Replace(" where ", " WHERE ", StringComparison.OrdinalIgnoreCase).Split(" WHERE ");
            inputs[0] = inputs[0].Trim();
            inputs[1] = inputs[1].Trim();

            if (inputs.Length > 2 || inputs[0].Length < 0 || inputs[1].Length < 0)
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            this.Service.UpdateRecords(GetDataForUpdate(inputs[0]), SearchingUtils.GetDataForFind(inputs[1], "and"));
        }
    }
}
