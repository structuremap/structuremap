using System;
using System.Collections.Generic;
using System.Resources;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class GraphLog
    {
        private List<Error> _errors = new List<Error>();
        private readonly List<Source> _sources = new List<Source>();
        private Source _currentSource;

        public void StartSource(string description)
        {
            Source source = new Source(description);
            _sources.Add(source);

            _currentSource = source;
        }

        public void RegisterError(Instance instance, int code, params object[] args)
        {
            Error error = new Error(code, args);
            error.Instance = instance.CreateToken();
            addError(error);
        }

        public void RegisterError(int code, params object[] args)
        {
            Error error = new Error(code, args);
            addError(error);
        }

        public void RegisterError(int code, Exception ex, params object[] args)
        {
            Error error = new Error(code, ex, args);
            addError(error);
        }


        private void addError(Error error)
        {
            error.Source = _currentSource;
            _errors.Add(error);
        }

        public void AssertHasError(int errorCode, string message)
        {
            Error error = Error.FromMessage(errorCode, message);
            if (!_errors.Contains(error))
            {
                string msg = "Did not have the requested Error.  Had:\n\n";
                foreach (Error err in _errors)
                {
                    msg += err.ToString() + "\n";
                }

                throw new ApplicationException(msg);
            }
        }

        public void AssertHasError(int errorCode)
        {
            foreach (Error error in _errors)
            {
                if (error.Code == errorCode)
                {
                    return;
                }
            }

            throw new ApplicationException("No error with code " + errorCode);
        }
    }

    public class Source
    {
        private string _description;

        public Source(string description)
        {
            _description = description;
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }

    public class PluginType : IEquatable<PluginType>
    {
        private readonly List<InstanceToken> _instances = new List<InstanceToken>();
        private readonly string _typeName;


        public PluginType(string fullName)
        {
            _typeName = fullName;
        }

        public PluginType(TypePath path) : this(path.AssemblyQualifiedName)
        {
        }

        public PluginType(Type pluginType) : this(pluginType.AssemblyQualifiedName)
        {
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        #region IEquatable<PluginType> Members

        public bool Equals(PluginType pluginType)
        {
            if (pluginType == null) return false;
            return Equals(_typeName, pluginType._typeName);
        }

        #endregion

        public void AddInstance(InstanceToken token)
        {
            if (!_instances.Contains(token))
            {
                _instances.Add(token);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as PluginType);
        }

        public override int GetHashCode()
        {
            return _typeName != null ? _typeName.GetHashCode() : 0;
        }
    }

    public class InstanceToken : IEquatable<InstanceToken>
    {
        private readonly string _description;
        private readonly string _name;
        private PluginType _pluginType;

        public InstanceToken(string name, string description, PluginType pluginType)
        {
            _name = name;
            _description = description;
            _pluginType = pluginType;
        }


        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }


        public PluginType PluginType
        {
            get { return _pluginType; }
        }

        #region IEquatable<InstanceToken> Members

        public bool Equals(InstanceToken instanceToken)
        {
            if (instanceToken == null) return false;
            if (!Equals(_name, instanceToken._name)) return false;
            if (!Equals(_description, instanceToken._description)) return false;
            if (!Equals(_pluginType, instanceToken._pluginType)) return false;
            return true;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Instance {0} ({1})", _name, _description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as InstanceToken);
        }

        public override int GetHashCode()
        {
            int result = _name != null ? _name.GetHashCode() : 0;
            result = 29*result + (_description != null ? _description.GetHashCode() : 0);
            result = 29*result + (_pluginType != null ? _pluginType.GetHashCode() : 0);
            return result;
        }
    }

    public class Error : IEquatable<Error>
    {
        private int _code;
        private string _message;
        private string _stackTrace = string.Empty;
        public InstanceToken Instance;
        public PluginType PluginType;
        public Source Source;


        private Error(int code, string message)
        {
            _code = code;
            _message = message;
        }

        public Error(int errorCode, params object[] args)
        {
            _code = errorCode;
            string template = getMessage(errorCode);
            if (template == null) template = string.Empty;

            _message = string.Format(template, args);
        }

        public Error(int errorCode, Exception ex, params object[] args) : this(errorCode, args)
        {
            _message += "\n\n" + ex.ToString();
            _stackTrace = ex.StackTrace;
        }


        public int Code
        {
            get { return _code; }
        }

        public Error(StructureMapException exception)
        {
            _code = exception.ErrorCode;
            _message = exception.Message;
            _stackTrace = exception.StackTrace;
        }

        private string getMessage(int errorCode)
        {
            ResourceManager resources = new ResourceManager(typeof(StructureMapException));
            return resources.GetString(errorCode.ToString());
        }


        public bool Equals(Error error)
        {
            if (error == null) return false;
            if (_code != error._code) return false;
            if (!Equals(_message, error._message)) return false;
            if (!Equals(_stackTrace, error._stackTrace)) return false;
            if (!Equals(Instance, error.Instance)) return false;
            if (!Equals(PluginType, error.PluginType)) return false;
            if (!Equals(Source, error.Source)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Error);
        }

        public override int GetHashCode()
        {
            int result = _code;
            result = 29*result + (_message != null ? _message.GetHashCode() : 0);
            result = 29*result + (_stackTrace != null ? _stackTrace.GetHashCode() : 0);
            result = 29*result + (Instance != null ? Instance.GetHashCode() : 0);
            result = 29*result + (PluginType != null ? PluginType.GetHashCode() : 0);
            result = 29*result + (Source != null ? Source.GetHashCode() : 0);
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
    }
}