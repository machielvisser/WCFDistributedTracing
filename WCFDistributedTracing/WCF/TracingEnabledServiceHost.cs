using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WCFDistributedTracing.WCF
{
    public class TracingEnabledServiceHost : ServiceHost
    {
        protected TracingEnabledServiceHost()
        {
        }

        public TracingEnabledServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
            var serviceDebugBehavior = Description.Behaviors
                .Where(behavior => behavior is ServiceDebugBehavior)
                .Select(behavior => behavior as ServiceDebugBehavior)
                .FirstOrDefault();

            if (serviceDebugBehavior != null)
                serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
        }

        public TracingEnabledServiceHost(object singletonInstance, params Uri[] baseAddresses) : base(singletonInstance, baseAddresses)
        {
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

            foreach (var endpoint in Description.Endpoints)
                endpoint.AddBehavior<InspectorBehavior<TracingInspector>>();
        }

        public override void AddServiceEndpoint(ServiceEndpoint endpoint)
        {
            endpoint.AddBehavior<InspectorBehavior<TracingInspector>>();

            base.AddServiceEndpoint(endpoint);
        }
    }
}
