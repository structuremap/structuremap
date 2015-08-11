using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL.Expressions
{
    public class ConfigureConventionExpression
    {
        private readonly ConfigurableRegistrationConvention _convention;

        internal ConfigureConventionExpression(ConfigurableRegistrationConvention convention)
        {
            _convention = convention;
        }

        public ConfigureConventionExpression OnAddedPluginTypes(Action<GenericFamilyExpression> configurePluginType)
        {
            _convention.SetFamilyConfigurationAction(configurePluginType);
            return this;
        }
    }
}