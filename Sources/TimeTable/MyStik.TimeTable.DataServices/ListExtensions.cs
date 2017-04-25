using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.DataServices
{
    /// <summary>
    /// Quelle:
    /// http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
    /// http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
    /// </summary>
    static class ListExtensions
    {
        static readonly Random Random = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}