using System;

namespace StructureMap.Configuration.Tokens.Properties
{
    [Serializable]
    public class TemplateProperty : GraphObject, IProperty
    {
        public static TemplateProperty[] GetTemplateProperties(InstanceMemento memento, TemplateToken token)
        {
            TemplateProperty[] returnValue = new TemplateProperty[token.Properties.Length];

            int i = 0;
            foreach (string templatePropertyName in token.Properties)
            {
                try
                {
                    string propertyValue = memento.GetProperty(templatePropertyName);
                    returnValue[i++] = new TemplateProperty(templatePropertyName, propertyValue);
                }
                catch (Exception ex)
                {
                    returnValue[i++] = new TemplateProperty(templatePropertyName, ex);
                }
            }

            return returnValue;
        }

        private string _propertyName;
        private string _propertyValue = string.Empty;

        public TemplateProperty(string propertyName, string propertyValue)
        {
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        public TemplateProperty(string propertyName, Exception ex)
        {
            _propertyName = propertyName;

            Problem problem = new Problem(ConfigurationConstants.MISSING_TEMPLATE_VALUE, ex);
            LogProblem(problem);
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public string PropertyValue
        {
            get { return _propertyValue; }
        }

        public Type PropertyType
        {
            get { return typeof (string); }
        }

        public void Validate(IInstanceValidator validator)
        {
            // no-op;
        }


        protected override string key
        {
            get { return PropertyName; }
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleTemplateProperty(this);
        }
    }
}