using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace WCFDistributedTracing.OpenTelemetry
{
    public static class ExtensionsMethods
    {
        public static IEnumerable<T> YieldHeader<T>(this MessageHeaders messageHeaders, string key)
        {
            if (messageHeaders.FindHeader(key, string.Empty) != -1)
                yield return messageHeaders.GetHeader<T>(key, string.Empty);
            else
                yield return default;
        }
    }
}
