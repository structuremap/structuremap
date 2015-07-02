using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Testing.Examples.FamilyPolicies
{
    // SAMPLE: SomeSettings
    public class SomeSettings
    {
        public string ThisDirectory { get; set; }
        public string ThatDirectory { get; set; }
    }

    // ENDSAMPLE

    // SAMPLE: ISettingsProvider
    public interface ISettingsProvider
    {
        T SettingsFor<T>() where T : class, new();

        object SettingsFor(Type settingsType);
    }

    public class AppSettingsProvider : ISettingsProvider
    {
        public T SettingsFor<T>() where T : class, new()
        {
            return SettingsFor(typeof (T)).As<T>();
        }

        public object SettingsFor(Type settingsType)
        {
            // The real one reads key/value data from
            // the appSettings and uses FubuCore's
            // model binding to assign data to a new
            // object of settingsType
            return null;
        }
    }

    // ENDSAMPLE

    // SAMPLE: SettingPolicy
    public class SettingPolicy : IFamilyPolicy
    {
        public PluginFamily Build(Type type)
        {
            if (type.Name.EndsWith("Settings") && type.IsConcreteWithDefaultCtor())
            {
                var family = new PluginFamily(type);
                var instance = buildInstanceForType(type);
                family.SetDefault(instance);

                return family;
            }

            return null;
        }

        public bool AppliesToHasFamilyChecks
        {
            get { return true; }
        }


        private static Instance buildInstanceForType(Type type)
        {
            var instanceType = typeof (SettingsInstance<>).MakeGenericType(type);
            var instance = Activator.CreateInstance(instanceType).As<Instance>();
            return instance;
        }
    }

    // SettingsInstance just uses the registered service for ISettingsProvider to
    // build the real object
    public class SettingsInstance<T> : LambdaInstance<T> where T : class, new()
    {
        public SettingsInstance() : base("Building {0} from application settings".ToFormat(typeof (T).FullName),
            c => c.GetInstance<ISettingsProvider>().SettingsFor<T>())
        {
        }
    }

    // ENDSAMPLE


    // SAMPLE: registering-custom-family-policy
    public class SettingsRegistry : Registry
    {
        public SettingsRegistry()
        {
            For<ISettingsProvider>().Use<AppSettingsProvider>();
            Policies.OnMissingFamily<SettingPolicy>();
        }
    }

    // ENDSAMPLE
}