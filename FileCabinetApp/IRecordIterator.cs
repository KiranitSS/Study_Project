using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents the structure of basic iterators.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Retrieves the next item in the collection.
        /// </summary>
        /// <returns>Returns next item from collection.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Shows whether the collection has more items.
        /// </summary>
        /// <returns>Returns if the collection has more items.</returns>
        public bool HasMore();
    }
}
