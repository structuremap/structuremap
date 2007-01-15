using StructureMap.DataAccess;
using StructureMap.DataAccess.MSSQL;

namespace StructureMap.Testing.DataAccess
{
	public class ObjectMother
	{
		private ObjectMother()
		{
		}


		public static MSSQLDatabaseEngine MSSQLDatabaseEngine()
		{
			string connectionString = "Data Source=localhost;Initial Catalog=StructureMap;user=dev1;password=dev1";
			return new MSSQLDatabaseEngine(connectionString);
		}

		public static DataSession MSSQLDataSession()
		{
			return new DataSession(MSSQLDatabaseEngine());
		}
	}
}
