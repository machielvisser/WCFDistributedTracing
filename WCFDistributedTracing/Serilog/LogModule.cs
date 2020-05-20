using Autofac;
using AutofacSerilogIntegration;
using Serilog;
using SerilogWeb.Classic;
using Module = Autofac.Module;

namespace WCFDistributedTracing.Serilog
{
    public class LogModule: Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Default seq url")]
        public LogModule()
        {
        }

        public LogModule(ILogger logger)
        {
            Log.Logger = logger;
            SerilogWebClassic.Configure(cfg => cfg.UseLogger(logger));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterLogger();
            base.Load(builder);
        }
    }
}
