using System;
using System.IO;

namespace StructureMap.Diagnostics
{
    public class Error : IEquatable<Error>
    {
        private readonly string _message;
        private string _stackTrace = string.Empty;
        public InstanceToken Instance;
        public string Source;

        public Error(string message)
        {
            _message = message;
        }

        public Error(Exception exception)
        {
            _message = exception.Message;

            writeStackTrace(exception);
        }

        public string Message
        {
            get { return _message; }
        }


        #region IEquatable<Error> Members

        public bool Equals(Error error)
        {
            if (error == null) return false;
            if (!Equals(_message, error._message)) return false;
            if (!Equals(_stackTrace, error._stackTrace)) return false;
            if (!Equals(Instance, error.Instance)) return false;
            return true;
        }

        #endregion

        private void writeStackTrace(Exception exception)
        {
            _stackTrace = string.Empty;
            var ex = exception;
            while (ex != null)
            {
                _stackTrace += exception.ToString();
                _stackTrace += "\n\n";
                ex = ex.InnerException;
            }
        }




        public override string ToString()
        {
            return string.Format("Error {0}", _message);
        }

        public void Write(StringWriter writer)
        {
            writer.WriteLine("Error:  ");
            if (Instance != null) writer.WriteLine(Instance.ToString());
            writer.WriteLine("Source:  " + Source);

            if (!string.IsNullOrEmpty(_message)) writer.WriteLine(_message);
            if (!string.IsNullOrEmpty(_stackTrace)) writer.WriteLine(_stackTrace);
        }
    }
}