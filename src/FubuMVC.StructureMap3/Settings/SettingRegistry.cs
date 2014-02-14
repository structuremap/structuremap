using System;
using FubuCore;
using FubuCore.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap3.Settings
{
    public class SettingRegistry : Registry
    {
        public SettingRegistry()
        {
            For<ISettingsProvider>().Use<SettingsProvider>();
            For<ISettingsSource>().Add(new AppSettingsSettingSource(SettingCategory.core));
        }

        public void AddSettingType<T>() where T : class, new()
        {
            ForSingletonOf<T>().UseInstance(new SettingsInstance<T>());
        }

        public void AddSettingType(Type type)
        {
            var instanceType = typeof (SettingsInstance<>).MakeGenericType(type);
            var instance = Activator.CreateInstance(instanceType).As<Instance>();

            For(type).Singleton().Use(instance);
        }
    }
}