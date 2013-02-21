using System;
using StructureMap.Diagnostics;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class ReferencedInstance : Instance, IEquatable<ReferencedInstance>
    {
        private readonly string _referenceKey;


        public ReferencedInstance(string referenceKey)
        {
            IsReference = true;
            CopyAsIsWhenClosingInstance = true;

            if (string.IsNullOrEmpty(referenceKey))
            {
                throw new ArgumentNullException("referenceKey");
            }

            _referenceKey = referenceKey;
        }


        public string ReferenceKey { get { return _referenceKey; } }

        protected override bool doesRecordOnTheStack { get { return false; } }

        #region IEquatable<ReferencedInstance> Members

        public bool Equals(ReferencedInstance referencedInstance)
        {
            if (referencedInstance == null) return false;
            return Equals(_referenceKey, referencedInstance._referenceKey);
        }

        #endregion

        protected override object build(Type pluginType, BuildSession session)
        {
            return session.CreateInstance(pluginType, _referenceKey);
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


        protected override Instance findMasterInstance(PluginFamily family, string profileName, GraphLog log)
        {
            Instance instance = family.GetInstance(_referenceKey);

            if (instance == null)
            {
                log.RegisterError(196, ReferenceKey, family.PluginType, profileName);
            }

            return instance;
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