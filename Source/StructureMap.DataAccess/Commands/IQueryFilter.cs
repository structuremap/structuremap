using System.Data;

namespace StructureMap.DataAccess.Commands
{
    [PluginFamily]
    public interface IQueryFilter : IParameter
    {
        void Initialize(IDatabaseEngine engine, IDbCommand command);
        bool IsActive();
        string GetWhereClause();
        void AttachParameters(IDbCommand command);
    }
}