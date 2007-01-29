using System;
using System.Data;
using System.Text;

namespace StructureMap.DataAccess
{
    [Serializable]
    public class CommandFailureException : ApplicationException
    {
        private string _message = string.Empty;
        private string _commandName = string.Empty;

        public CommandFailureException()
        {
        }

        public CommandFailureException(IDbCommand command, Exception innerException) : base("", innerException)
        {
            buildMessage(command);
        }

        public CommandFailureException(string commandName, IDbCommand command, Exception innerException)
            : base("", innerException)
        {
            buildMessage(command);
        }

        public string CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }

        private void buildMessage(IDbCommand command)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append("Command Failure!\r\n");
            SB.Append(InnerException.Message);
            SB.Append("\r\n");


            SB.Append("Database Command:\r\n");
            addProperty(SB, "CommandText", command.CommandText);
            addProperty(SB, "TimeOut", command.CommandTimeout.ToString());
            addProperty(SB, "CommandType", command.CommandType.ToString("G"));
            if (null != command.Connection)
            {
                addProperty(SB, "ConnectionString", command.Connection.ConnectionString);
            }
            else
            {
                addProperty(SB, "Connection (and ConnectionString)", "null");
            }
            foreach (IDbDataParameter param in command.Parameters)
            {
                if (param.Value != null)
                {
                    addProperty(SB, param.ParameterName, param.Value.ToString());
                }
                else
                {
                    addProperty(SB, param.ParameterName, "NULL");
                }
            }

            _message = SB.ToString();
        }

        private void addProperty(StringBuilder SB, string name, string propertyValue)
        {
            SB.Append(name);
            SB.Append(":  ");
            SB.Append(propertyValue);
            SB.Append("\n");
        }

        public override sealed string Message
        {
            get { return string.Format("Command:  {0}\n\r{1}", _commandName, _message); }
        }
    }
}