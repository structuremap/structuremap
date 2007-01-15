using System;
using NUnit.Framework;
using StructureMap.DataAccess.Commands;

namespace StructureMap.Testing.DataAccess.Commands
{
	[TestFixture]
	public class TemplatedQueryFilterTester
	{
		
		private TemplatedQueryFilter _filter;

		[SetUp]
		public void SetUp()
		{
			_filter = new TemplatedQueryFilter("Name", "Column = '{Value}'");
		}

		[Test]
		public void GetAndSetProperty()
		{
			Assert.IsNull(_filter.GetProperty());
			_filter.SetProperty("Jeremy");
			Assert.AreEqual("Jeremy", _filter.GetProperty());
		}

		[Test]
		public void IsActive()
		{
			Assert.IsNull(_filter.GetProperty());
			Assert.IsFalse(_filter.IsActive());
			
			_filter.SetProperty("Jeremy");
			Assert.IsTrue(_filter.IsActive());
		}


		[Test]
		public void GetWhereClause()
		{
			_filter.SetProperty("Jeremy");
			Assert.AreEqual("Column = 'Jeremy'", _filter.GetWhereClause());
		}


	}
}
