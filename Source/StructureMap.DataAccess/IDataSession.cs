using System.Data;

namespace StructureMap.DataAccess
{
	public enum StructureMapCommandType
	{
		Templated,
		Parameterized,
		StoredProcedure
	}
	
	[PluginFamily("Default")]
	public interface IDataSession
	{
		bool IsInTransaction { get; }

		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();

		int ExecuteCommand(IDbCommand command);
		int ExecuteSql(string sql);
		IDataReader ExecuteReader(IDbCommand command);
		IDataReader ExecuteReader(string sql);
		DataSet ExecuteDataSet(IDbCommand command);
		DataSet ExecuteDataSet(string sql);
		object ExecuteScalar(IDbCommand command);
		object ExecuteScalar(string sql);

		ICommandCollection Commands { get; }
		IReaderSourceCollection ReaderSources { get; }
		
		IReaderSource CreateReaderSource(StructureMapCommandType commandType, string commandText);
		ICommand CreateCommand(StructureMapCommandType commandType, string commandText);

		void Initialize(IInitializable initializable);
	}
}