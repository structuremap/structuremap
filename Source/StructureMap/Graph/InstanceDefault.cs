using System;
using StructureMap.Configuration;

namespace StructureMap.Graph
{
    /// <summary>
    /// Stores the default instance key for a PluginType.  Member of the <see cref="Profile"/>
    /// and <see cref="MachineOverride"/> classes
    /// </summary>
    [Serializable]
    public class InstanceDefault : GraphObject, ICloneable
    {
        private string _defaultKey;
        private string _pluginTypeName;

        public InstanceDefault(string pluginTypeName, string defaultKey) : base()
        {
            _pluginTypeName = pluginTypeName;
            _defaultKey = defaultKey;
        }

        public InstanceDefault(Type pluginType, string defaultKey) : this(pluginType.FullName, defaultKey)
        {
        }

        public string PluginTypeName
        {
            get { return _pluginTypeName; }
        }

        /// <summary>
        /// Default instance key
        /// </summary>
        public string DefaultKey
        {
            get { return _defaultKey; }
            set { _defaultKey = value; }
        }

        protected override string key
        {
            get { return PluginTypeName; }
        }

        #region ICloneable Members

        public object Clone()
        {
            object clone = MemberwiseClone();
            return clone;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            InstanceDefault instanceDefault = obj as InstanceDefault;
            if (instanceDefault == null) return false;
            return
                Equals(_pluginTypeName, instanceDefault._pluginTypeName) &&
                Equals(_defaultKey, instanceDefault._defaultKey);
        }

        public override int GetHashCode()
        {
            return
                (_pluginTypeName != null ? _pluginTypeName.GetHashCode() : 0) +
                29*(_defaultKey != null ? _defaultKey.GetHashCode() : 0);
        }
    }
}