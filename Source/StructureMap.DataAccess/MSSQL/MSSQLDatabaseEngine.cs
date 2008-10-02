using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace StructureMap.DataAccess.MSSQL
{
    [Pluggable("MSSQL")]
    public class MSSQLDatabaseEngine : IDatabaseEngine
    {
        private readonly string _connectionString;

        [DefaultConstructor]
        public MSSQLDatabaseEngine(IConnectionStringProvider provider)
        {
            _connectionString = provider.GetConnectionString();
        }

        public MSSQLDatabaseEngine(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region IDatabaseEngine Members

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbCommand GetCommand()
        {
            return new SqlCommand();
        }

        public IDbDataAdapter GetDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public IDataParameter CreateStringParameter(string parameterName, int size, bool isNullable)
        {
            SqlParameter parameter = createParameter(parameterName);

            parameter.DbType = DbType.String;
            parameter.Size = size;
            parameter.IsNullable = isNullable;

            return parameter;
        }

        public IDataParameter CreateParameter(string parameterName, DbType dbType, bool isNullable)
        {
            SqlParameter parameter = createParameter(parameterName);

            parameter.DbType = dbType;
            parameter.IsNullable = isNullable;

            return parameter;
        }

        public string GetParameterName(string logicalName)
        {
            return "@" + logicalName;
        }

        public IDbCommand CreateStoredProcedureCommand(string commandText)
        {
            SqlCommand command = null;
            var connection = new SqlConnection(_connectionString);

            try
            {
                command = new SqlCommand(commandText, connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlCommandBuilder.DeriveParameters(command);

                pruneDuplicates(command);

                // There are no InputOutput parameters in T-SQL
                foreach (IDataParameter param in command.Parameters)
                {
                    if (param.Direction == ParameterDirection.InputOutput)
                    {
                        param.Direction = ParameterDirection.Output;
                    }
                }
            }
            catch (SqlException e)
            {
                var ex = new Exception("Error connecting or executing database command " + e.Message);
                throw (ex);
            }
            finally
            {
                command.Connection = null;
                connection.Close();
            }


            return command;
        }

        #endregion

        public static IDataSession BuildSession(string connectionString)
        {
            return new DataSession(new MSSQLDatabaseEngine(connectionString));
        }

        private SqlParameter createParameter(string logicalName)
        {
            var parameter = new SqlParameter();
            parameter.ParameterName = "@" + logicalName;
            return parameter;
        }

        public void TestConnection()
        {
            throw new NotImplementedException();
        }

        private void pruneDuplicates(SqlCommand command)
        {
            var list = new ArrayList();

            var parameters = new SqlParameter[command.Parameters.Count];
            command.Parameters.CopyTo(parameters, 0);

            foreach (SqlParameter parameter in parameters)
            {
                string parameterName = parameter.ParameterName.ToUpper();
                if (list.Contains(parameterName))
                {
                    command.Parameters.Remove(parameter);
                }
                else
                {
                    list.Add(parameterName);
                }
            }
        }
    }
}