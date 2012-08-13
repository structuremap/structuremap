using System;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class NotExecutedCommandException : ApplicationException
    {
        private readonly string _parameterName;
        private string _commandName = string.Empty;

        public NotExecutedCommandException(string parameterName)
        {
            _parameterName = parameterName;
        }

        public NotExecutedCommandException(string parameterName, string commandName)
        {
            _parameterName = parameterName;
            _commandName = commandName;
        }

        public string CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }

        public override string Message
        {
            get
            {
                return
                    string.Format("Tried to access parameter {0} of command {1} before it was executed.", _parameterName,
                                  _commandName);
            }
        }
    }
}