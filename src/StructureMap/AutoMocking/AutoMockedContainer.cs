using System;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.AutoMocking
{
    /// <summary>
    /// Special version of a StructureMap Container that is typically
    /// used for "auto mocked" tests
    /// </summary>
    public class AutoMockedContainer : Container, IFamilyPolicy
    {
        private readonly ServiceLocator _locator;


        /// <summary>
        /// Creates a new AutoMockedContainer using the supplied ServiceLocator
        /// </summary>
        /// <param name="locator"></param>
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

        PluginFamily IFamilyPolicy.Build(Type pluginType)
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

        bool IFamilyPolicy.AppliesToHasFamilyChecks
        {
            get { return true; }
        }
    }
}