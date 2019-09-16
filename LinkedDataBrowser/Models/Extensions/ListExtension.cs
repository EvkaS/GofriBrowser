using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedDataBrowser.Models.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// Get shorter list, keep only selected subset of elements
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="count">Maximum count of the elements to keep</param>
        /// <param name="n">Order of the subset to keep in the list</param>
        /// <param name="indexOfSubset"> Index in the original array where the subset starts</param>
        /// <returns></returns>
        public static List<T> GetNthSubset<T>(this List<T> originalList, int count, int n, out int indexOfSubset)
        {
            indexOfSubset = -1;
            if (n < 1) throw new ArgumentException("Order of subset must be at least 1 ");
            if (count < 0) throw new ArgumentException("Cannot select negative number of elements ");
            else if (count == 0) return new List<T>();
            if (originalList.Count == 0) return null;

            int startIndex = (n - 1) * count;
            if (startIndex >= originalList.Count)
                return null;
            indexOfSubset = startIndex;

            int tillEnd = originalList.Count - startIndex;
            int newCount = (tillEnd < count) ? tillEnd : count;
            return originalList.GetRange(startIndex, newCount);
        }
    }
}
