using System;

namespace StructureMap.Pipeline
{
    public class BuildFrame
    {
        private readonly Type _requestedType;
        private readonly string _name;
        private readonly Type _concreteType;

        public BuildFrame(Type requestedType, string name, Type concreteType)
        {
            _requestedType = requestedType;
            _name = name;
            _concreteType = concreteType;
        }

        public Type RequestedType
        {
            get { return _requestedType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Type ConcreteType
        {
            get { return _concreteType; }
        }
    }
}