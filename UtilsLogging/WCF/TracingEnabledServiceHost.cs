using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace UtilsLogging.WCF
{
    public class TracingDuplexClientBase<T> : DuplexClientBase<T> where T : class
    {
        public TracingDuplexClientBase(object callbackInstance) : base(callbackInstance)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance) : base(callbackInstance)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
            Endpoint.AddTracingBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
            Endpoint.AddTracingBehavior();
        }
    }

    public class TracingEnabledServiceHost: System.ServiceModel.ServiceHost
    {
        protected TracingEnabledServiceHost()
        {
        }

        public TracingEnabledServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }

        public TracingEnabledServiceHost(object singletonInstance, params Uri[] baseAddresses) : base(singletonInstance, baseAddresses)
        {
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            foreach (var endpoint in Description.Endpoints)
            {
                endpoint.AddTracingBehavior();
            }
        }
    }
}
