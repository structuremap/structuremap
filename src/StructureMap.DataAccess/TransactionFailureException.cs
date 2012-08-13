using System;

namespace StructureMap.DataAccess
{
    /// <summary>
    /// A custom exception in the event of a database Commit or Rollback
    /// failing
    /// </summary>
    public class TransactionFailureException : ApplicationException
    {
        public TransactionFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}