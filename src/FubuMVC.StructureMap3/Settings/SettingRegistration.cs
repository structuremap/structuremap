using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using StructureMap.Configuration.DSL;

namespace FubuMVC.StructureMap3.Settings
{
    [ConfigurationType(ConfigurationType.Policy)]
    public class SettingRegistration : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<ConfigurationSettings>();

            var registry = settings.BuildRegistry(graph);

            graph.Services.AddService(typeof(Registry), ObjectDef.ForValue(registry));
        }
    }
}