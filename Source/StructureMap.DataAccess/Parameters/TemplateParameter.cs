using System;
using System.Data;
using System.Text;

namespace StructureMap.DataAccess.Parameters
{
    public class TemplateParameter : IParameter
    {
        private readonly string _parameterName;
        private object _value;

        public TemplateParameter(string parameterName)
        {
            _parameterName = parameterName;
        }

        public void SetProperty(object propertyValue)
        {
            _value = propertyValue;
        }

        public object GetProperty()
        {
            return _value;
        }

        public string ParameterName
        {
            get { return _parameterName; }
        }

        public void Substitute(StringBuilder sb)
        {
            if (_value == null)
            {
                string message = string.Format("The parameter {0} is null.", _parameterName);
                throw new InvalidOperationException(message);
            }

            string token = "{" + _parameterName + "}";
            sb.Replace(token, _value.ToString());
        }

        public void OverrideParameterType(DbType dbtype)
        {
            // no-op;
        }
    }
}