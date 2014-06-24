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
            if (pluginType.IsConcrete() && pluginType.GetConstructors().All(x => x.GetParameters().Count() != 0))
            {
                return null;
            }

            var family = new PluginFamily(pluginType);
            family.SetDefault(() => {
                var service = _locator.Service(pluginType);

                return new ObjectInstance(service);
            });

            return family;
        }

        public bool AppliesToHasFamilyChecks
        {
            get { return false; }
        }
    }
}