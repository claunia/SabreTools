using System.Collections.Generic;

namespace SabreTools.Core
{
    public static class ConcurrentListExtensions
    {
        public static ConcurrentList<T> ToConcurrentList<T>(this IEnumerable<T> values)
        {
            var list = new ConcurrentList<T>();
            list.SetInternalList([.. values]);
            return list;
        }
    }
}
