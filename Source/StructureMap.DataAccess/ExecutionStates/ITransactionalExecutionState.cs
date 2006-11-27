using System;

namespace StructureMap.DataAccess.ExecutionStates
{
	public interface ITransactionalExecutionState : IExecutionState, IDisposable
	{
		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();
	}
}