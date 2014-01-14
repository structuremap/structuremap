using System;
using System.IO;

namespace StructureMap.Diagnostics
{
    public class Error : IEquatable<Error>
    {
        [Obsolete("Remove the error code")]
        private readonly int _code;
        private readonly string _message;
        private string _stackTrace = string.Empty;
        public InstanceToken Instance;
        public string Source;

        [Obsolete("Remove the error code")]
        private Error(int code, string message)
        {

            _code = code;
            _message = message;
        }

        [Obsolete("Remove the error code")]
        public Error(int errorCode, params object[] args)
        {
            _code = errorCode;
            _message = ErrorMessages.GetMessage(errorCode, args);
        }

        [Obsolete("Remove the error code")]
        public Error(int errorCode, Exception ex, params object[] args)
            : this(errorCode, args)
        {
            _message += "\n\n" + ex.Message;

            writeStackTrace(ex);
        }


        public Error(StructureMapException exception)
        {
            _message = exception.Message;

            writeStackTrace(exception);
        }


        public int Code
        {
            get { return _code; }
        }

        #region IEquatable<Error> Members

        public bool Equals(Error error)
        {
            if (error == null) return false;
            if (_code != error._code) return false;
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Error);
        }

        public override int GetHashCode()
        {
            var result = _code;
            result = 29*result + (_message != null ? _message.GetHashCode() : 0);
            result = 29*result + (_stackTrace != null ? _stackTrace.GetHashCode() : 0);
            result = 29*result + (Instance != null ? Instance.GetHashCode() : 0);
            return result;
        }


        public override string ToString()
        {
            return string.Format("Error {0} -- {1}", _code, _message);
        }

        public static Error FromMessage(int code, string message)
        {
            return new Error(code, message);
        }

        public void Write(StringWriter writer)
        {
            writer.WriteLine("Error:  " + Code);
            if (Instance != null) writer.WriteLine(Instance.ToString());
            writer.WriteLine("Source:  " + Source);

            if (!string.IsNullOrEmpty(_message)) writer.WriteLine(_message);
            if (!string.IsNullOrEmpty(_stackTrace)) writer.WriteLine(_stackTrace);
        }
    }
}