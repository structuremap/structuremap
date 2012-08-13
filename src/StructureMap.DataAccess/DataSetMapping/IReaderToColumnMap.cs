using System.Data;

namespace StructureMap.DataAccess.DataSetMapping
{
    public interface IReaderToColumnMap
    {
        void Initialize(DataTable table, IDataReader reader);
        void Fill(DataRow row, IDataReader reader);
    }
}