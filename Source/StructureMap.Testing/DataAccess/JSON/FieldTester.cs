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
            DataTable table = new DataTable();
            table.Columns.Add("theName", typeof (string));
            DataRow row = table.Rows.Add(new object[] {"Jeremy"});


            Field field = new Field(0, "theName");
            JSONObject o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: 'Jeremy'}", o.ToJSON());
        }

        [Test]
        public void BasicWriteWithNull()
        {
            DataTable table = new DataTable();
            table.Columns.Add("theName", typeof (string));
            DataRow row = table.Rows.Add(new object[] {DBNull.Value});

            Field field = new Field(0, "theName");
            JSONObject o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: null}", o.ToJSON());
        }

        [Test]
        public void NumericField()
        {
            DataTable table = new DataTable();
            table.Columns.Add("theName", typeof (int));
            DataRow row = table.Rows.Add(new object[] {34});

            Field field = new NumericField(0, "theName");
            JSONObject o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: 34}", o.ToJSON());
        }

        [Test]
        public void DateTimeField()
        {
            DataTable table = new DataTable();
            table.Columns.Add("theName", typeof (DateTime));
            DataRow row = table.Rows.Add(new object[] {new DateTime(2003, 4, 24, 1, 2, 3)});


            Field field = new DateTimeField(0, "theName");
            JSONObject o = new JSONObject();
            field.Write(o, row);

            Assert.AreEqual("{theName: new Date(2003, 4, 24, 1, 2, 3)}", o.ToJSON());
        }
    }
}