using System;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.MSSQL;
using StructureMap.DataAccess.Parameterization;

namespace StructureMap.Testing.DataAccess
{
	[TestFixture]
	public class Debugging
	{

		[Test, Explicit]
		public void GiveJSONAWhirl()
		{
			DataSession session = new DataSession(new MSSQLDatabaseEngine("Data Source=sddev2-sql01;Initial Catalog=einvoice;user=sa;password=datacert"));
			
			ParameterizedCommand command = new ParameterizedCommand("select top 100 *  from system_health");
			command.Attach(session);
			
			string json = command.ExecuteJSON();
			
			Debug.WriteLine(json);
		}
	}
}
