using System.Data;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess
{
    [PluginFamily]
    public interface IReaderSource
    {
        string Name { get; set; }

        [IndexerName("Parameter")]
        object this[string parameterName] { get; set; }

        IDataReader ExecuteReader();
        DataSet ExecuteDataSet();
        object ExecuteScalar();

        void Attach(IDataSession session);

        string ExecuteJSON();
    }
}