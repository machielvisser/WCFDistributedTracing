using OpenTelemetry.Trace;
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

        public static void PutPeerNameAttribute(this TelemetrySpan telemetrySpan, string peerName)
        {
            telemetrySpan.SetAttribute("peer.name", peerName);
        }

        public static void PutPeerPortAttribute(this TelemetrySpan telemetrySpan, int peerPort)
        {
            telemetrySpan.SetAttribute("peer.port", peerPort);
        }

        public static void PutNetHostNameAttribute(this TelemetrySpan telemetrySpan, string netHostName)
        {
            telemetrySpan.SetAttribute("net.host.name", netHostName);
        }

        public static void PutWcfServiceAttribute(this TelemetrySpan telemetrySpan, string service)
        {
            telemetrySpan.SetAttribute("peer.service", service);
            telemetrySpan.SetAttribute("wcf.service", service);
        }

        public static void PutWcfServiceNamespaceAttribute(this TelemetrySpan telemetrySpan, string serviceNamespace)
        {
            telemetrySpan.SetAttribute("wcf.servicenamespace", serviceNamespace);
        }

        public static void PutWcfOperationAttribute(this TelemetrySpan telemetrySpan, string operation)
        {
            telemetrySpan.SetAttribute("wcf.operation", operation);
        }

        public static void PutWcfIsOneWayAttribute(this TelemetrySpan telemetrySpan, bool isOneWay)
        {
            telemetrySpan.SetAttribute("wcf.isoneway", isOneWay);
        }
    }
}
