using System;
using System.Data;

namespace StructureMap.DataAccess.DataSetMapping
{
    public class ReaderToColumnMap : IReaderToColumnMap
    {
        private readonly string _columnName;
        private readonly string _readerName;
        private DataColumn _column;
        private int _ordinal;

        public ReaderToColumnMap(string readerName, string columnName)
        {
            _readerName = readerName;
            _columnName = columnName;
        }

        #region IReaderToColumnMap Members

        public void Initialize(DataTable table, IDataReader reader)
        {
            _column = table.Columns[_columnName];
            _ordinal = reader.GetOrdinal(_readerName);

            if (_ordinal < 0)
            {
                throw new ArgumentOutOfRangeException("reader",
                                                      "The IDataReader does not have an Ordinal for " + _readerName);
            }

            if (_column == null)
            {
                throw new ArgumentOutOfRangeException("table",
                                                      "The DataTable does not have a DataColumn named " + _columnName);
            }
        }

        public void Fill(DataRow row, IDataReader reader)
        {
            object dataValue = getRawValue(reader);
            row[_column] = dataValue;
        }

        #endregion

        protected virtual object getRawValue(IDataReader reader)
        {
            return reader.GetValue(_ordinal);
        }
    }
}