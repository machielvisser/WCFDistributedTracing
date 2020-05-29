using System.Collections.Generic;
using System.Linq;

namespace WCFDistributedTracing.WCF
{
    public static class ExtensionMethods
    {
        public static void AddIfNotExists<T, U>(this ICollection<U> collection, T element) where T : U
        {
            if (!collection.OfType<T>().Any())
                collection.Add(element);
        }
    }
}
