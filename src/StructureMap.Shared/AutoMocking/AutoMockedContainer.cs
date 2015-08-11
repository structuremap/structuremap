using System;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.TypeRules;
using StructureMap.Pipeline;

namespace StructureMap.AutoMocking
{
    public class AutoMockedContainer : Container, IFamilyPolicy
    {
        private readonly ServiceLocator _locator;


        public AutoMockedContainer(ServiceLocator locator)
        {
            nameContainer(this);

            Configure(x => x.Policies.OnMissingFamily(this));

            _locator = locator;
        }

        private void nameContainer(IContainer container)
        {
            container.Name = "AutoMocking-" + container.Name;
        }

        public PluginFamily Build(Type pluginType)
        {
            if (pluginType.IsConcrete())
            {
                return null;
            }

            var family = new PluginFamily(pluginType);

            var service = _locator.Service(pluginType);

            var instance = new ObjectInstance(service);

            family.SetDefault(instance);

            return family;
        }

        public bool AppliesToHasFamilyChecks
        {
            get { return true; }
        }
    }
}