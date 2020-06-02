using System.Collections.Generic;

namespace WCFDistributedTracing.OpenTelemetry
{
    public static class ExtensionsMethods
    {
        public static IEnumerable<T> Yield<T>(this T input)
        {
            yield return input;
        }
    }
}
