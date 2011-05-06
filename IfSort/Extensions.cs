using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IfSort
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> handle)
        {
            foreach(var item in items)
            {
                handle(item);
            }
        }

        public static string JoinToString(this IEnumerable enumerable, string separator)
        {
            return string.Join(separator, enumerable
                                              .Cast<object>()
                                              .Select(o => o != null
                                                               ? o.ToString()
                                                               : "(null)"));
        }
    }
}