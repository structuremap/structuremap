using System;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class ParameterValidationFailureException : ApplicationException
    {
        private string _message = string.Empty;

        public override string Message
        {
            get { return _message; }
        }

        public void MarkWrongParameterValue(string parameterName, object expected, object actual)
        {
            _message +=
                string.Format("Wrong value for Parameter '{0}', expected '{1}', actual '{2}'\n\r", parameterName,
                              expected, actual);
        }

        public void MarkUnexpectedParameter(string parameterName, object actual)
        {
            _message += string.Format("Unexpected Parameter for '{0}' = '{1}'\n\r", parameterName, actual);
        }

        public void MarkMissingParameter(string parameterName, object expected)
        {
            _message += string.Format("Missing Parameter Value for '{0}', expected '{1}'\n\r", parameterName, expected);
        }

        public void ThrowIfExceptions()
        {
            if (_message != string.Empty)
            {
                throw this;
            }
        }
    }
}