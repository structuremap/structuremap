using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class ReferencedInstance : Instance, IEquatable<ReferencedInstance>
    {
        private readonly string _referenceKey;

        public ReferencedInstance(string referenceKey)
        {
            if (referenceKey.IsEmpty())
            {
                throw new ArgumentNullException("referenceKey");
            }

            _referenceKey = referenceKey;
        }


        public string ReferenceKey
        {
            get { return _referenceKey; }
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

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new ReferencedDependencySource(pluginType, _referenceKey);
        }

        protected override string getDescription()
        {
            return string.Format("\"{0}\"", _referenceKey);
        }

        public override Instance CloseType(Type[] types)
        {
            return this;
        }
    }
}