using System;
using Serilog.Core;
using Serilog.Events;

namespace WCFDistributedTracing.Serilog
{
    /// <summary>Enrich log events with a HttpRequestId GUID.</summary>
    public class WCFTracingEnricher : ILogEventEnricher
    {
        /// <summary>The property name added to enriched log events.</summary>
        public const string TraceIdPropertyName = "TraceId";

        /// <summary>
        /// Enrich the log event with an id assigned to the currently-executing HTTP request, if any.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));
            var property = new LogEventProperty(TraceIdPropertyName, new ScalarValue(DistributedOperationContext.Current?.TraceId ?? "-"));
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
