using Autofac;
using AutofacSerilogIntegration;
using Serilog;
using SerilogWeb.Classic;
using Module = Autofac.Module;

namespace UtilsLogging
{
    public class SerilogModule: Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Default seq url")]
        public SerilogModule()
        {
        }

        public SerilogModule(ILogger logger)
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
