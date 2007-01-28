using System;

namespace StructureMap.Configuration.Tokens.Properties
{
    [Serializable]
    public class DefaultChildPropertyMode : IChildPropertyMode
    {
        private readonly ChildProperty _property;

        public DefaultChildPropertyMode(ChildProperty property)
        {
            _property = property;
        }

        public void Validate(IInstanceValidator validator)
        {
            if (!validator.HasDefaultInstance(_property.PluginType))
            {
                string message =
                    string.Format("There is not a default instance for type {0} configured in StructureMap",
                                  _property.PluginType);
                Problem problem = new Problem(ConfigurationConstants.NO_DEFAULT_INSTANCE_CONFIGURED, message);
                _property.LogProblem(problem);
            }
        }

        public void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleDefaultChildProperty(_property);
        }
    }
}