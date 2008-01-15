using System.Data;

namespace StructureMap.DataAccess.Parameters
{
    public class Parameter : IParameter
    {
        private readonly IDataParameter _innerParameter;
        private string _parameterName;

        public Parameter(IDataParameter innerParameter) : this(innerParameter, innerParameter.ParameterName)
        {
        }

        public Parameter(IDataParameter innerParameter, string parameterName)
        {
            _innerParameter = innerParameter;
            _parameterName = parameterName;
        }

        /// <summary>
        /// Only exposed for unit testing
        /// </summary>
        public IDataParameter InnerParameter
        {
            get { return _innerParameter; }
        }

        #region IParameter Members

        public void SetProperty(object propertyValue)
        {
            _innerParameter.Value = propertyValue;
        }

        public object GetProperty()
        {
            return _innerParameter.Value;
        }

        public string ParameterName
        {
            get { return _parameterName; }
        }

        public void OverrideParameterType(DbType dbtype)
        {
            _innerParameter.DbType = dbtype;
        }

        #endregion
    }
}