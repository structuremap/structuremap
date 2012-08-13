using System.Data;

namespace StructureMap.DataAccess.Commands
{
    [Pluggable("Templated")]
    public class TemplatedQueryFilter : QueryFilter
    {
        public TemplatedQueryFilter(string parameterName, string sqlSnippet) : base(parameterName, sqlSnippet)
        {
        }


        public override void Initialize(IDatabaseEngine engine, IDbCommand command)
        {
            // no-op
        }

        public override string GetWhereClause()
        {
            return _sqlSnippet.Replace(REPLACEMENT_VALUE, _innerValue.ToString());
        }

        public override void AttachParameters(IDbCommand command)
        {
            // no-op
        }
    }
}