using System.Data;

namespace StructureMap.DataAccess.Parameterization
{
    [Pluggable("Basic")]
    public class BasicParameterTemplate : IParameterTemplate
    {
        private readonly DbType _dbType;
        private readonly bool _isNullable;
        private readonly string _parameterName;

        public BasicParameterTemplate(string parameterName, DbType dbType, bool isNullable)
        {
            _parameterName = parameterName;
            _dbType = dbType;
            _isNullable = isNullable;
        }

        public BasicParameterTemplate(string parameterName)
            : this(parameterName, DbType.Object, true)
        {
            _parameterName = parameterName;
        }

        #region IParameterTemplate Members

        public IDataParameter ConfigureParameter(IDatabaseEngine database)
        {
            return database.CreateParameter(_parameterName, _dbType, _isNullable);
        }

        public string ParameterName
        {
            get { return _parameterName; }
        }

        #endregion
    }
}