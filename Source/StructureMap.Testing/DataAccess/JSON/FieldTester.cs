using System;
using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess.JSON;

namespace StructureMap.Testing.DataAccess.JSON
{
    [TestFixture]
    public class FieldTester
    {
        [Test]
        public void BasicWriteWithAValue()
        {
            var table = new DataTable();
            table.Columns.Add("theName", typeof (string));
            DataRow row = table.Rows.Add(new object[] {"Jeremy"});


            var field = new Field(0, "theName");
            var o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: 'Jeremy'}", o.ToJSON());
        }

        [Test]
        public void BasicWriteWithNull()
        {
            var table = new DataTable();
            table.Columns.Add("theName", typeof (string));
            DataRow row = table.Rows.Add(new object[] {DBNull.Value});

            var field = new Field(0, "theName");
            var o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: null}", o.ToJSON());
        }

        [Test]
        public void DateTimeField()
        {
            var table = new DataTable();
            table.Columns.Add("theName", typeof (DateTime));
            DataRow row = table.Rows.Add(new object[] {new DateTime(2003, 4, 24, 1, 2, 3)});


            Field field = new DateTimeField(0, "theName");
            var o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: new Date(2003, 4, 24, 1, 2, 3)}", o.ToJSON());
        }

        [Test]
        public void NumericField()
        {
            var table = new DataTable();
            table.Columns.Add("theName", typeof (int));
            DataRow row = table.Rows.Add(new object[] {34});

            Field field = new NumericField(0, "theName");
            var o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: 34}", o.ToJSON());
        }
    }
}