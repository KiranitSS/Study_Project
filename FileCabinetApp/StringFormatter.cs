using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Convert string to another types.
    /// </summary>
    public static class StringFormatter
    {
        /// <summary>
        /// Convert string to fixed size char array .
        /// </summary>
        /// <param name="value">Value for covertation.</param>
        /// <param name="size">Size of new array.</param>
        /// <returns>Return fixed size char array.</returns>
        public static List<char> StringToChar(string value, int size)
        {
            if (value is null)
            {
                return new List<char>();
            }

            char[] chars = new char[size];

            for (int i = 0; i < size && i < value.Length; i++)
            {
                chars[i] = value[i];
            }

            return chars.ToList();
        }
    }
}
