using System.Collections.Generic;

namespace Grp2
{
    public static class LinqExtensions
    {
        public static HashSet<T> ToHashSet<T>(
            this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
        public static SortedSet<T> ToSortedSet<T>(
            this IEnumerable<T> source)
        {
            return new SortedSet<T>(source);
        }
    }
}
