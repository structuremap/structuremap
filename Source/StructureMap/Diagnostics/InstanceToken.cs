using System;

namespace StructureMap.Diagnostics
{
    public class InstanceToken : IEquatable<InstanceToken>
    {
        private readonly string _description;
        private readonly string _name;

        public InstanceToken(string name, string description)
        {
            _name = name;
            _description = description;
        }


        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        #region IEquatable<InstanceToken> Members

        public bool Equals(InstanceToken instanceToken)
        {
            if (instanceToken == null) return false;
            if (!Equals(_name, instanceToken._name)) return false;
            if (!Equals(_description, instanceToken._description)) return false;
            return true;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Instance '{0}' ({1})", _name, _description);
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
            return result;
        }
    }
}