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
		private string _pluginTypeName;
		private string _defaultKey;

		public InstanceDefault(string pluginTypeName, string defaultKey) : base()
		{
			_pluginTypeName = pluginTypeName;
			_defaultKey = defaultKey;
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

		#region ICloneable Members

		public object Clone()
		{
			object clone = this.MemberwiseClone();
			return clone;
		}

		#endregion

		public override int GetHashCode()
		{
			return _pluginTypeName.GetHashCode();
		}

		protected override string key
		{
			get { return this.PluginTypeName; }
		}
	}
}