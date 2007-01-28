using System;

namespace StructureMap.Configuration.Tokens.Properties
{
    [Serializable]
    public class PrimitiveProperty : Property
    {
        private string _value;

        public PrimitiveProperty(PropertyDefinition definition, InstanceMemento memento) : base(definition)
        {
            try
            {
                _value = memento.GetProperty(definition.PropertyName);

                Type targetType = definition.PropertyType;
                Convert.ChangeType(_value, targetType);
            }
            catch (FormatException ex)
            {
                Problem problem = new Problem(ConfigurationConstants.INVALID_PROPERTY_CAST, ex);
                LogProblem(problem);
            }
            catch (Exception ex)
            {
                Problem problem = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, ex);
                LogProblem(problem);
            }
        }

        public string Value
        {
            get { return _value; }
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandlePrimitiveProperty(this);
        }
    }
}