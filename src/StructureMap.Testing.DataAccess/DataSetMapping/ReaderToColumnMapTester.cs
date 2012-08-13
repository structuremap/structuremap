using System;
using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess.DataSetMapping;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess.DataSetMapping
{
	[TestFixture]
	public class ReaderToColumnMapTester
	{
		private DataTable _sourceTable;
		private DataTable _destinationTable;
		private DataRow _row;
		private IDataReader _reader;
		private DateTime _texasDate = new DateTime(1846, 12, 29);

		[SetUp]
		public void SetUp()
		{
			_sourceTable = new DataTable();
			_sourceTable.Columns.Add("State", typeof(string));
			_sourceTable.Columns.Add("StateHoodDate", typeof(DateTime));
			_sourceTable.Columns.Add("Population", typeof(long));

			_sourceTable.Rows.Add(new object[]{"Texas", _texasDate, 5});
			_sourceTable.Rows.Add(new object[]{"Puerto Rico", DBNull.Value, 6});
		
			_destinationTable = new DataTable();
			_destinationTable.Columns.Add("StateName", typeof(string));
			_destinationTable.Columns.Add("AdmissionDate", typeof(DateTime));
			_destinationTable.Columns.Add("Residents", typeof(long));

			_row = _destinationTable.Rows.Add(new object[]{"Missouri", DateTime.Today, 2});

			_reader = new TableDataReader(_sourceTable);
		}

		[Test]
		public void TransferString()
		{
			ReaderToColumnMap map = new ReaderToColumnMap("State", "StateName");
			map.Initialize(_destinationTable, _reader);

			// Move to first row
			_reader.Read();

			map.Fill(_row, _reader);

			Assert.AreEqual("Texas", (string)_row["StateName"]);
		}

		[Test]
		public void TransferDate()
		{
			ReaderToColumnMap map = new ReaderToColumnMap("StatehoodDate", "AdmissionDate");
			map.Initialize(_destinationTable, _reader);

			// Move to first row
			_reader.Read();

			map.Fill(_row, _reader);

			Assert.AreEqual(_texasDate, (DateTime)_row["AdmissionDate"]);
		}

		[Test]
		public void TransferLong()
		{
			ReaderToColumnMap map = new ReaderToColumnMap("Population", "Residents");
			map.Initialize(_destinationTable, _reader);

			// Move to first row
			_reader.Read();

			map.Fill(_row, _reader);

			Assert.AreEqual(5, (long)_row["Residents"]);
		}

		[Test]
		public void TransferADBNull()
		{
			ReaderToColumnMap map = new ReaderToColumnMap("StatehoodDate", "AdmissionDate");
			map.Initialize(_destinationTable, _reader);

			// Move to second row
			_reader.Read();
			_reader.Read();

			map.Fill(_row, _reader);

			Assert.AreEqual(DBNull.Value, _row["AdmissionDate"]);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void InitializeWithAnInvalidReaderNameAndThrowAnException()
		{
			ReaderToColumnMap map = new ReaderToColumnMap("NotARealColumn", "AdmissionDate");
			map.Initialize(_destinationTable, _reader);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void InitializeWithAnInvalidColumnNameAndThrowAnException()
		{
			ReaderToColumnMap map = new ReaderToColumnMap("StatehoodDate", "NotARealColumn");
			map.Initialize(_destinationTable, _reader);
		}
	}
}
