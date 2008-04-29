using System;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class ReferencedInstance : Instance, IEquatable<ReferencedInstance>
    {
        private readonly string _referenceKey;


        public ReferencedInstance(string referenceKey)
        {
            // TODO:  VALIDATION if referenceKey is null or empty
            _referenceKey = referenceKey;
        }


        public string ReferenceKey
        {
            get { return _referenceKey; }
        }

        protected override object build(Type pluginType, IInstanceCreator creator)
        {
            return creator.CreateInstance(pluginType, _referenceKey);
        }


        public bool Equals(ReferencedInstance referencedInstance)
        {
            if (referencedInstance == null) return false;
            return Equals(_referenceKey, referencedInstance._referenceKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ReferencedInstance);
        }

        public override int GetHashCode()
        {
            return _referenceKey != null ? _referenceKey.GetHashCode() : 0;
        }


        protected override Instance findMasterInstance(PluginFamily family)
        {
            // TODO:  Sad Path
            return family.GetInstance(_referenceKey);
        }
    }
}