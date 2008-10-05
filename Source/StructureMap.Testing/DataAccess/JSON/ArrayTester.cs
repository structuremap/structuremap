using NUnit.Framework;
using StructureMap.DataAccess.JSON;

namespace StructureMap.Testing.DataAccess.JSON
{
    [TestFixture]
    public class ArrayTester
    {
        [Test]
        public void ToJSON()
        {
            JSONObject o1 = new JSONObject().AddNumber("age", 32);
            JSONObject o2 = new JSONObject().AddNumber("age", 35);
            JSONObject o3 = new JSONObject().AddNumber("age", 2);

            var jsonArray = new JSONArray(false);
            Assert.AreEqual("[]", jsonArray.ToJSON());

            jsonArray.AddObject(o1);
            Assert.AreEqual("[{age: 32}]", jsonArray.ToJSON());

            jsonArray.AddObject(o2);
            Assert.AreEqual("[{age: 32}, {age: 35}]", jsonArray.ToJSON());

            jsonArray.AddObject(o3);
            Assert.AreEqual("[{age: 32}, {age: 35}, {age: 2}]", jsonArray.ToJSON());
        }
    }
}