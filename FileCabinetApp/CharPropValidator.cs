using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CharPropValidator : IRecordValidator
    {
        private readonly bool mustBeLetter;

        public CharPropValidator(bool mustBeLetter)
        {
            this.mustBeLetter = mustBeLetter;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return this.ValidateCharProp(parameters.CharProp);
        }

        /// <summary>
        /// Checks ability to add simple char property.
        /// </summary>
        /// <param name="charProp">Simple char property.</param>
        /// <returns>Returns ability to add simple char property to record.</returns>
        private bool ValidateCharProp(char charProp)
        {
            if (char.IsWhiteSpace(charProp))
            {
                return false;
            }

            if (this.mustBeLetter && !char.IsLetter(charProp))
            {
                Console.WriteLine("Must be letter!");
                return false;
            }

            return true;
        }
    }
}
