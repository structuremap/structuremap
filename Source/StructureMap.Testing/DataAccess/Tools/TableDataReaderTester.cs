using System;
using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess.Tools
{
    [TestFixture]
    public class TableDataReaderTester
    {
        [Test]
        public void ClosingFunctionality()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Rows.Add(new object[] {"TX"});
            table.Rows.Add(new object[] {"MO"});
            table.Rows.Add(new object[] {"AR"});

            TableDataReader reader = new TableDataReader(table);

            Assert.IsFalse(reader.IsClosed);
            reader.Close();
            Assert.IsTrue(reader.IsClosed);

            reader = new TableDataReader(table);
            reader.Dispose();
            Assert.IsTrue(reader.IsClosed);
        }

        [Test]
        public void FieldCount()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Columns.Add("Direction", typeof (string));
            table.Columns.Add("Count", typeof (int));
            table.Columns.Add("Sum", typeof (int));

            IDataReader reader = new TableDataReader(table);

            Assert.AreEqual(4, reader.FieldCount);
        }

        [Test]
        public void GetOrdinal()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Columns.Add("Direction", typeof (string));
            table.Columns.Add("Count", typeof (int));
            table.Columns.Add("Sum", typeof (int));

            IDataReader reader = new TableDataReader(table);

            Assert.AreEqual(0, reader.GetOrdinal("State"));
            Assert.AreEqual(1, reader.GetOrdinal("Direction"));
            Assert.AreEqual(2, reader.GetOrdinal("Count"));
            Assert.AreEqual(3, reader.GetOrdinal("Sum"));
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void GetOrdinalWhenTheOrdinalCannotBeFound()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Columns.Add("Direction", typeof (string));
            table.Columns.Add("Count", typeof (int));
            table.Columns.Add("Sum", typeof (int));

            IDataReader reader = new TableDataReader(table);

            Assert.AreEqual(0, reader.GetOrdinal("NotAColumn"));
        }

        [Test]
        public void GetValue()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Columns.Add("Direction", typeof (string));
            table.Columns.Add("Count", typeof (int));
            table.Columns.Add("Sum", typeof (int));

            table.Rows.Add(new object[] {"TX", "North", 1, 2});

            IDataReader reader = new TableDataReader(table);
            reader.Read();

            Assert.AreEqual("TX", reader.GetValue(0));
            Assert.AreEqual("North", reader.GetValue(1));
            Assert.AreEqual(1, (int) reader.GetValue(2));
            Assert.AreEqual(2, (int) reader.GetValue(3));
        }

        [Test]
        public void IsDBNull()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Columns.Add("Direction", typeof (string));
            table.Columns.Add("Count", typeof (int));
            table.Columns.Add("Sum", typeof (int));

            table.Rows.Add(new object[] {DBNull.Value, "North", DBNull.Value, 2});

            IDataReader reader = new TableDataReader(table);
            reader.Read();

            Assert.IsTrue(reader.IsDBNull(0));
            Assert.IsFalse(reader.IsDBNull(1));
            Assert.IsTrue(reader.IsDBNull(2));
            Assert.IsFalse(reader.IsDBNull(3));
        }

        [Test, ExpectedException(typeof (ApplicationException))]
        public void ReadingAClosedReaderThrowsAnException()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Rows.Add(new object[] {"TX"});
            table.Rows.Add(new object[] {"MO"});
            table.Rows.Add(new object[] {"AR"});

            TableDataReader reader = new TableDataReader(table);

            Assert.IsFalse(reader.IsClosed);
            reader.Close();
            Assert.IsTrue(reader.IsClosed);

            reader.Read();
        }

        [Test]
        public void ReadWithNoRows()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));

            TableDataReader reader = new TableDataReader(table);

            // No rows, so it should return false
            Assert.IsFalse(reader.Read());
        }

        [Test]
        public void ReadWithOneRow()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Rows.Add(new object[] {"TX"});

            TableDataReader reader = new TableDataReader(table);

            // One row, so it should return true the first time, and false the second
            Assert.IsTrue(reader.Read());
            Assert.IsFalse(reader.Read());
        }

        [Test]
        public void ReadWithThreeRows()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Rows.Add(new object[] {"TX"});
            table.Rows.Add(new object[] {"MO"});
            table.Rows.Add(new object[] {"AR"});

            TableDataReader reader = new TableDataReader(table);

            // One row, so it should return true the first time, and false the second
            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsFalse(reader.Read());
        }

        [Test]
        public void ReadWithTwoRows()
        {
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof (string));
            table.Rows.Add(new object[] {"TX"});
            table.Rows.Add(new object[] {"MO"});

            TableDataReader reader = new TableDataReader(table);

            // One row, so it should return true the first time, and false the second
            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsFalse(reader.Read());
        }
    }
}