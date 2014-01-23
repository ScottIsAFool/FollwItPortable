using System.Collections.Generic;
using System.Linq;

namespace FollwItPortable.Extensions
{
    internal static class CollectionExtensions
    {
        internal static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || !list.Any();
        }
    }
}
