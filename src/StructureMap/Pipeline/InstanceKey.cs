using System;

namespace StructureMap.Pipeline
{
    [Obsolete("Kill this whole silly thing")]
    [Serializable]
    public class InstanceKey
    {
        [Obsolete("Kill!")] private WeakReference _instance;
        [Obsolete("Kill!")] private WeakReference _session;

        public InstanceKey()
        {
        }

        public InstanceKey(Instance instance, Type pluginType)
        {
            Name = instance.Name;
            PluginType = pluginType;
        }

        public string Name { get; set; }
        public Type PluginType { get; set; }

        [Obsolete("Kill!")]
        public BuildSession Session { get { return (BuildSession) _session.Target; } set { _session = new WeakReference(value); } }

        [Obsolete("Kill!")]
        public Instance Instance
        {
            get { return (Instance) _instance.Target; }
            set
            {
                Name = value.Name;
                _instance = new WeakReference(value);
            }
        }

        public bool Equals(InstanceKey obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Name, Name) && Equals(obj.PluginType, PluginType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (InstanceKey)) return false;
            return Equals((InstanceKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^
                       (PluginType != null ? PluginType.GetHashCode() : 0);
            }
        }
    }
}