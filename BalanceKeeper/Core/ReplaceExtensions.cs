using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Core
{
    public static class ReplaceExtensions
    {
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> sequence, T find, T replaceWith)
        {
            return Replace(sequence, find, replaceWith, EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> sequence, T find, T replaceWith, IEqualityComparer<T> comparer)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (comparer == null) throw new ArgumentNullException("comparer");

            return ReplaceImpl(sequence, find, replaceWith, comparer);
        }

        private static IEnumerable<T> ReplaceImpl<T>(IEnumerable<T> sequence, T find, T replaceWith, IEqualityComparer<T> comparer)
        {
            foreach (T item in sequence)
            {
                bool match = comparer.Equals(find, item);
                T x = match ? replaceWith : item;
                yield return x;
            }
        }
    }
}
