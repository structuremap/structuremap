using FubuMVC.Core;

namespace FubuMVC.StructureMap3.Settings
{
    public class StructureMapExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Add<SettingRegistration>();
        }
    }
}