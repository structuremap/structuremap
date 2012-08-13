using System.Data;

namespace StructureMap.DataAccess.Commands
{
    [Pluggable("Parameterized")]
    public class ParameterizedQueryFilter : QueryFilter
    {
        private IDbDataParameter _innerParameter;

        public ParameterizedQueryFilter(string parameterName, string sqlSnippet) : base(parameterName, sqlSnippet)
        {
        }

        public override void Initialize(IDatabaseEngine engine, IDbCommand command)
        {
            string innerParameterName = engine.GetParameterName(ParameterName);
            sqlSnippet = sqlSnippet.Replace(REPLACEMENT_VALUE, innerParameterName);
            _innerParameter = command.CreateParameter();
            _innerParameter.ParameterName = innerParameterName;
        }

        public override string GetWhereClause()
        {
            return sqlSnippet;
        }

        public override void AttachParameters(IDbCommand command)
        {
            command.Parameters.Add(_innerParameter);
            _innerParameter.Value = GetProperty();
        }
    }
}