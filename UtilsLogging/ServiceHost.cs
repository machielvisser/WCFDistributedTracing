using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using UtilsLogging.Wcf;

namespace UtilsLogging
{
    public class TracingDuplexClientBase<T> : DuplexClientBase<T> where T : class
    {
        public TracingDuplexClientBase(object callbackInstance) : base(callbackInstance)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(object callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance) : base(callbackInstance)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }

        public TracingDuplexClientBase(InstanceContext callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
            Endpoint.AddWcfCorrelationBehavior();
        }
    }

    public class ServiceHost: System.ServiceModel.ServiceHost
    {
        protected ServiceHost()
        {
        }

        public ServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }

        public ServiceHost(object singletonInstance, params Uri[] baseAddresses) : base(singletonInstance, baseAddresses)
        {
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            foreach (var endpoint in Description.Endpoints)
            {
                endpoint.AddWcfCorrelationBehavior();
            }
        }
    }
}
