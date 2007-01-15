using System;
using NUnit.Framework;
using StructureMap.DataAccess.JSON;

namespace StructureMap.Testing.DataAccess.JSON
{
	[TestFixture]
	public class ObjectTester
	{
		[Test]
		public void Write()
		{
			JSONObject o = new JSONObject();
			Assert.AreEqual("{}", o.ToJSON());
			
			o.AddNull("name");
			Assert.AreEqual("{name: null}", o.ToJSON());
			
			o.AddNumber("age", 32);
			Assert.AreEqual("{name: null, age: 32}", o.ToJSON());
			
			o.AddString("state", "Texas");
			Assert.AreEqual("{name: null, age: 32, state: 'Texas'}", o.ToJSON());
			
			o.AddDate("time", new DateTime(2006, 7, 28));
			Assert.AreEqual("{name: null, age: 32, state: 'Texas', time: new Date(2006, 7, 28, 0, 0, 0)}", o.ToJSON());
			
		}
	}
}
