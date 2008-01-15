using System;
using System.Data;

namespace StructureMap.DataAccess.Tools
{
    public class TableDataReader : IDataReader
    {
        private int _currentPosition;
        private DataRow _currentRow;
        private DataTable _currentTable;
        private bool _isClosed;
        private int _tableIndex = -1;
        private DataTable[] _tables;

        public TableDataReader()
            : this(new DataTable())
        {
        }

        public TableDataReader(DataTable table)
            : this(new DataTable[] {table})
        {
        }

        public TableDataReader(DataTable[] tables)
        {
            _isClosed = false;
            _tables = tables;
            moveToNextTable();
        }

        #region IDataReader Members

        public void Close()
        {
            _isClosed = true;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            assertOpen();
            moveToNextRow();
            return (_currentRow != null);
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsClosed
        {
            get { return _isClosed; }
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            Close();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            return _currentRow[i];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            int index = _currentTable.Columns.IndexOf(name);

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("name", name, "This ordinal cannot be found.");
            }

            return index;
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return Convert.ToString(_currentRow[i]);
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return (_currentRow[i] == DBNull.Value);
        }

        public int FieldCount
        {
            get { return _currentTable.Columns.Count; }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }

        public object this[string name]
        {
            get
            {
                int index = GetOrdinal(name);
                return GetValue(index);
            }
        }

        #endregion

        private void moveToNextTable()
        {
            _tableIndex++;
            _currentTable = _tables[_tableIndex];
            _currentPosition = -1;
        }

        private void moveToNextRow()
        {
            _currentPosition++;
            _currentRow = _currentPosition < _currentTable.Rows.Count ? _currentTable.Rows[_currentPosition] : null;
        }

        private void assertOpen()
        {
            if (_isClosed)
            {
                throw new ApplicationException("This DataReader is closed!");
            }
        }
    }
}