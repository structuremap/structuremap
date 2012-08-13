using System.Data;

namespace StructureMap.DataAccess.Parameterization
{
    [Pluggable("String")]
    public class StringParameterTemplate : IParameterTemplate
    {
        private readonly bool _isNullable;
        private readonly string _parameterName;
        private readonly int _size;

        public StringParameterTemplate(string parameterName, int size, bool isNullable)
        {
            _parameterName = parameterName;
            _size = size;
            _isNullable = isNullable;
        }

        #region IParameterTemplate Members

        public IDataParameter ConfigureParameter(IDatabaseEngine database)
        {
            return database.CreateStringParameter(_parameterName, _size, _isNullable);
        }

        public string ParameterName
        {
            get { return _parameterName; }
        }

        #endregion
    }
}