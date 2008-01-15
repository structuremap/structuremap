using System.Data;

namespace StructureMap.DataAccess.Parameterization
{
    [PluginFamily]
    public interface IParameterTemplate
    {
        string ParameterName { get; }
        IDataParameter ConfigureParameter(IDatabaseEngine database);
    }
}