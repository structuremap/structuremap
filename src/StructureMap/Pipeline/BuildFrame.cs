using System;

namespace StructureMap.Pipeline
{
    public interface IBuildFrame
    {
        /// <summary>
        /// The requested PluginType of the Instance being create
        /// </summary>
        Type RequestedType { get; }

        /// <summary>
        /// The Name of the Instance being created
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The actual ConcreteType being created.  This will not always
        /// be available
        /// </summary>
        Type ConcreteType { get; }
    }

    /// <summary>
    /// Models the current place in an object graph during the construction of
    /// an instance.  Provides contextual information that can be used
    /// to alter the desired construction of child objects
    /// </summary>
    public class BuildFrame : IBuildFrame
    {
        private readonly Type _concreteType;
        private readonly string _name;
        private readonly Type _requestedType;
        private BuildFrame _next;
        private BuildFrame _parent;

        public BuildFrame(Type requestedType, string name, Type concreteType)
        {
            _requestedType = requestedType;
            _name = name;
            _concreteType = concreteType;
        }

        internal BuildFrame Parent { get { return _parent; } }

        /// <summary>
        /// The requested PluginType of the Instance being create
        /// </summary>
        public Type RequestedType { get { return _requestedType; } }

        /// <summary>
        /// The Name of the Instance being created
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// The actual ConcreteType being created.  This will not always
        /// be available
        /// </summary>
        public Type ConcreteType { get { return _concreteType; } }

        public void Attach(BuildFrame next)
        {
            _next = next;
            _next._parent = this;
        }

        internal BuildFrame Detach()
        {
            if (_parent != null) _parent._next = null;
            return _parent;
        }

        public override string ToString()
        {
            return string.Format("RequestedType: {0}, Name: {1}, ConcreteType: {2}", _requestedType, _name,
                                 _concreteType);
        }

        public bool Equals(BuildFrame obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._requestedType, _requestedType) && Equals(obj._name, _name) &&
                   Equals(obj._concreteType, _concreteType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BuildFrame)) return false;
            return Equals((BuildFrame) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_requestedType != null ? _requestedType.GetHashCode() : 0);
                result = (result*397) ^ (_name != null ? _name.GetHashCode() : 0);
                result = (result*397) ^ (_concreteType != null ? _concreteType.GetHashCode() : 0);
                return result;
            }
        }

        public bool Contains(BuildFrame frame)
        {
            if (_requestedType == frame._requestedType && _name == frame._name)
            {
                return true;
            }

            return _next == null ? false : _next.Contains(frame);
        }

        public string ToStackString()
        {
            string message = "\n1.) " + ToString();
            BuildFrame next = _next;

            int i = 2;
            while (next != null)
            {
                message += "\n{0}.) {1}".ToFormat(i++, next);
                next = next._next;
            }

            return message;
        }
    }
}