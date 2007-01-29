using System;
using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess.DataSetMapping;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess.DataSetMapping
{
    [TestFixture]
    public class ReaderToTableMapperTester
    {
        private DataTable _sourceTable;
        private DataTable _destinationTable;
        private IDataReader _reader;
        private DateTime _texasDate = new DateTime(1846, 12, 29);
        private DateTime _missouriDate = new DateTime(1821, 8, 10);

        [SetUp]
        public void SetUp()
        {
            _sourceTable = new DataTable();
            _sourceTable.Columns.Add("State", typeof (string));
            _sourceTable.Columns.Add("StateHoodDate", typeof (DateTime));
            _sourceTable.Columns.Add("Population", typeof (long));
            _sourceTable.Columns.Add("BeenThere", typeof (string));

            _sourceTable.Rows.Add(new object[] {"Texas", _texasDate, 5, "Y"});
            _sourceTable.Rows.Add(new object[] {"Puerto Rico", DBNull.Value, 6, "N"});
            _sourceTable.Rows.Add(new object[] {"Missouri", _missouriDate, 7, "Y"});

            _destinationTable = new DataTable();
            _destinationTable.Columns.Add("StateName", typeof (string));
            _destinationTable.Columns.Add("AdmissionDate", typeof (DateTime));
            _destinationTable.Columns.Add("Residents", typeof (long));
            _destinationTable.Columns.Add("BeenThere", typeof (bool));

            _reader = new TableDataReader(_sourceTable);
        }

        [Test]
        public void WriteRowsToDataTable()
        {
            ReaderToTableMapper mapper = new ReaderToTableMapper(
                new IReaderToColumnMap[]
                    {
                        new ReaderToColumnMap("State", "StateName"),
                        new ReaderToColumnMap("StateHoodDate", "AdmissionDate"),
                        new ReaderToColumnMap("Population", "Residents"),
                        new YesNoReaderToColumnMap("BeenThere", "BeenThere")
                    });

            mapper.Fill(_destinationTable, _reader);

            Assert.AreEqual(3, _destinationTable.Rows.Count);

            Assert.AreEqual("Texas", _destinationTable.Rows[0][0]);
            Assert.AreEqual(_texasDate, _destinationTable.Rows[0][1]);
            Assert.AreEqual(5, _destinationTable.Rows[0][2]);
            Assert.AreEqual(true, _destinationTable.Rows[0][3]);

            Assert.AreEqual("Puerto Rico", _destinationTable.Rows[1][0]);
            Assert.AreEqual(DBNull.Value, _destinationTable.Rows[1][1]);
            Assert.AreEqual(6, _destinationTable.Rows[1][2]);
            Assert.AreEqual(false, _destinationTable.Rows[1][3]);

            Assert.AreEqual("Missouri", _destinationTable.Rows[2][0]);
            Assert.AreEqual(_missouriDate, _destinationTable.Rows[2][1]);
            Assert.AreEqual(7, _destinationTable.Rows[2][2]);
            Assert.AreEqual(true, _destinationTable.Rows[2][3]);


            Assert.IsNull(_destinationTable.GetChanges());
        }
    }
}