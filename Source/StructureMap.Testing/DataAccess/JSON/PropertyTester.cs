using System;
using NUnit.Framework;
using StructureMap.DataAccess.JSON;

namespace StructureMap.Testing.DataAccess.JSON
{
	[TestFixture]
	public class PropertyTester
	{
		[Test]
		public void NumberProperty()
		{
			Part part = JSONProperty.Number("theName", 345.32);
			Assert.AreEqual("theName: 345.32", part.ToJSON());
		}

		[Test]
		public void StringProperty()
		{
			Part part = JSONProperty.String("theName", "Jeremy");
			Assert.AreEqual("theName: 'Jeremy'", part.ToJSON());
		}

		[Test]
		public void NullProperty()
		{
			Part part = JSONProperty.Null("theName");
			Assert.AreEqual("theName: null", part.ToJSON());
		}

		[Test]
		public void DateTimeProperty()
		{
			Part part = JSONProperty.DateTime("theName", new DateTime(2006, 2, 3, 5, 4, 3));
			Assert.AreEqual("theName: new Date(2006, 2, 3, 5, 4, 3)", part.ToJSON());
		}
	}
}
