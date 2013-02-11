using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class GraphLog
    {
        private readonly List<Error> _errors = new List<Error>();
        private readonly List<string> _sources = new List<string>();
        private string _currentSource;

        public int ErrorCount { get { return _errors.Count; } }

        public string[] Sources { get { return _sources.ToArray(); } }

        public void StartSource(string description)
        {
            _currentSource = description;
            _sources.Add(description);
        }

        public void RegisterError(IDiagnosticInstance instance, int code, params object[] args)
        {
            var error = new Error(code, args);
            error.Instance = instance.CreateToken();
            addError(error);
        }

        public void RegisterError(int code, params object[] args)
        {
            var error = new Error(code, args);
            addError(error);
        }

        public void RegisterError(int code, Exception ex, params object[] args)
        {
            var error = new Error(code, ex, args);
            addError(error);
        }


        public void RegisterError(StructureMapException ex)
        {
            var error = new Error(ex);
            addError(error);
        }

        private void addError(Error error)
        {
            error.Source = _currentSource;
            _errors.Add(error);
        }

        public void AssertFailures()
        {
            if (_errors.Count == 0)
            {
                return;
            }

            string message = BuildFailureMessage();

            throw new StructureMapConfigurationException(message);
        }

        public string BuildFailureMessage()
        {
            if (_errors.Count == 0)
                return "No Errors";
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            writer.WriteLine("StructureMap configuration failures:");

            foreach (Error error in _errors)
            {
                error.Write(writer);
                writer.WriteLine(
                    "-----------------------------------------------------------------------------------------------------");
                writer.WriteLine();
                writer.WriteLine();
            }

            return sb.ToString();
        }

        public void AssertHasError(int errorCode, string message)
        {
            Error error = Error.FromMessage(errorCode, message);
            if (!_errors.Contains(error))
            {
                string msg = "Did not have the requested Error.  Had:\n\n";
                foreach (Error err in _errors)
                {
                    msg += err + "\n";
                }

                throw new ApplicationException(msg);
            }
        }

        public void AssertHasError(int errorCode)
        {
            string message = "No error with code " + errorCode + "\nHad errors: ";
            foreach (Error error in _errors)
            {
                message += error.Code + ", ";
                if (error.Code == errorCode)
                {
                    return;
                }
            }

            throw new ApplicationException(message);
        }

        public void AssertHasNoError(int errorCode)
        {
            foreach (Error error in _errors)
            {
                if (error.Code == errorCode)
                {
                    throw new ApplicationException("Has error " + errorCode);
                }
            }
        }

        public void WithType(TypePath path, string context, Action<Type> action)
        {
            try
            {
                Type type = path.FindType();
                action(type);
            }
            catch (StructureMapException ex)
            {
                RegisterError(ex);
            }
            catch (Exception ex)
            {
                RegisterError(131, ex, path.AssemblyQualifiedName, context);
            }
        }

        public TryAction Try(Action action)
        {
            return new TryAction(action, this);
        }

        #region Nested type: TryAction

        public class TryAction
        {
            private readonly Action _action;
            private readonly GraphLog _log;

            internal TryAction(Action action, GraphLog log)
            {
                _action = action;
                _log = log;
            }

            public void AndReportErrorAs(int code, params object[] args)
            {
                try
                {
                    _action();
                }
                catch (Exception ex)
                {
                    _log.RegisterError(code, ex, args);
                }
            }

            public void AndLogAnyErrors()
            {
                try
                {
                    _action();
                }
                catch (StructureMapException ex)
                {
                    _log.RegisterError(ex);
                }
                catch (Exception ex)
                {
                    _log.RegisterError(400, ex);
                }
            }
        }

        #endregion
    }
}