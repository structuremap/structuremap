using System.Data;

namespace StructureMap.DataAccess.Commands
{
    public abstract class QueryFilter : IQueryFilter
    {
        private readonly string _parameterName;
        protected string _sqlSnippet;
        protected object _innerValue = null;
        protected const string REPLACEMENT_VALUE = "{Value}";


        public QueryFilter(string parameterName, string sqlSnippet)
        {
            _parameterName = parameterName;
            _sqlSnippet = sqlSnippet;
        }

        protected string sqlSnippet
        {
            get { return _sqlSnippet; }
            set { _sqlSnippet = value; }
        }

        public void SetProperty(object propertyValue)
        {
            _innerValue = propertyValue;
        }

        public object GetProperty()
        {
            return _innerValue;
        }

        public abstract void Initialize(IDatabaseEngine engine, IDbCommand command);

        public bool IsActive()
        {
            return _innerValue != null;
        }

        public abstract string GetWhereClause();

        public abstract void AttachParameters(IDbCommand command);

        public string ParameterName
        {
            get { return _parameterName; }
        }

        public void OverrideParameterType(DbType dbtype)
        {
            // no-op;
        }
    }
}