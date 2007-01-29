using System.Data;

namespace StructureMap.DataAccess.Parameterization
{
    [PluginFamily]
    public interface IParameterTemplate
    {
        IDataParameter ConfigureParameter(IDatabaseEngine database);
        string ParameterName { get; }
    }
}