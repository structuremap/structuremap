using System.Data;

namespace StructureMap.DataAccess.DataSetMapping
{
    public class ReaderToTableMapper
    {
        private readonly IReaderToColumnMap[] _columnMaps;

        public ReaderToTableMapper(IReaderToColumnMap[] columnMaps)
        {
            _columnMaps = columnMaps;
        }

        public void Fill(DataTable table, IDataReader reader)
        {
            foreach (IReaderToColumnMap columnMap in _columnMaps)
            {
                columnMap.Initialize(table, reader);
            }

            while (reader.Read())
            {
                DataRow row = table.NewRow();
                table.Rows.Add(row);

                foreach (IReaderToColumnMap columnMap in _columnMaps)
                {
                    columnMap.Fill(row, reader);
                }
            }

            table.AcceptChanges();
        }
    }
}