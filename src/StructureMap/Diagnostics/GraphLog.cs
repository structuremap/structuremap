using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class GraphLog
    {
        private readonly List<Error> _errors = new List<Error>();
        private readonly List<string> _sources = new List<string>();
        private string _currentSource;

        public int ErrorCount
        {
            get { return _errors.Count; }
        }

        public string[] Sources
        {
            get { return _sources.ToArray(); }
        }

        public void StartSource(string description)
        {
            _currentSource = description;
            _sources.Add(description);
        }

        public void RegisterError(string template, params object[] args)
        {
            var error = new Error(template.ToFormat(args));
            addError(error);
        }

        public void RegisterError(Exception ex)
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
            if (!_errors.Any())
            {
                return;
            }

            var message = BuildFailureMessage();

            throw new StructureMapConfigurationException(message);
        }

        public string BuildFailureMessage()
        {
            if (!_errors.Any())
                return "No Errors";

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            writer.WriteLine("StructureMap configuration failures:");

            foreach (var error in _errors)
            {
                error.Write(writer);
                writer.WriteLine(
                    "-----------------------------------------------------------------------------------------------------");
                writer.WriteLine();
                writer.WriteLine();
            }

            return sb.ToString();
        }

        public IEnumerable<Error> Errors
        {
            get { return _errors; }
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
                    _log.RegisterError(ex);
                }
            }

            public void AndLogAnyErrors()
            {
                try
                {
                    _action();
                }
                catch (Exception ex)
                {
                    _log.RegisterError(ex);
                }
            }
        }

        #endregion
    }
}