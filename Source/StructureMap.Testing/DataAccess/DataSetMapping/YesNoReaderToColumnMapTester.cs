using System;
using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess.DataSetMapping;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess.DataSetMapping
{
    [TestFixture]
    public class YesNoReaderToColumnMapTester
    {
        private DataTable _table;
        private DataTable _destinationTable;
        private DataRow _row;
        private IDataReader _reader;
        private YesNoReaderToColumnMap _map;


        [SetUp]
        public void SetUp()
        {
            _table = new DataTable();
            _table.Columns.Add("Active", typeof (string));
            _table.Rows.Add(new object[] {"Y"});
            _table.Rows.Add(new object[] {"N"});
            _table.Rows.Add(new object[] {DBNull.Value});
            _table.Rows.Add(new object[] {"A"});

            _destinationTable = new DataTable();
            _destinationTable.Columns.Add("Active", typeof (bool));
            _row = _destinationTable.Rows.Add(new object[] {false});

            _reader = new TableDataReader(_table);

            _map = new YesNoReaderToColumnMap("Active", "Active");
            _map.Initialize(_destinationTable, _reader);
        }

        [Test]
        public void MapYesValue()
        {
            // position on row #1
            _reader.Read();

            _row["Active"] = false;
            _map.Fill(_row, _reader);

            Assert.AreEqual(true, _row["Active"]);
        }


        [Test]
        public void MapNoValue()
        {
            // position on row #2
            _reader.Read();
            _reader.Read();

            _row["Active"] = true;
            _map.Fill(_row, _reader);

            Assert.AreEqual(false, _row["Active"]);
        }


        [Test]
        public void MapDBNullValue()
        {
            // position on row #3
            _reader.Read();
            _reader.Read();
            _reader.Read();

            _row["Active"] = true;
            _map.Fill(_row, _reader);

            Assert.AreEqual(DBNull.Value, _row["Active"]);
        }


        [Test, ExpectedException(typeof (ApplicationException))]
        public void MapInvalidValue()
        {
            // position on row #4
            _reader.Read();
            _reader.Read();
            _reader.Read();
            _reader.Read();

            _row["Active"] = true;
            _map.Fill(_row, _reader);
        }
    }
}