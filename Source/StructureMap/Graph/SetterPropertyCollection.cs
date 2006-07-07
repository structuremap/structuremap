using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using StructureMap.Attributes;

namespace StructureMap.Graph
{
	/// <summary>
	/// Custom collection class for SetterProperty objects
	/// </summary>
	public class SetterPropertyCollection : PluginGraphObjectCollection
	{
		private readonly Plugin _plugin;
		private ListDictionary _setterProperties;

		public SetterPropertyCollection(Plugin plugin) : base(null)
		{
			_setterProperties = new ListDictionary();
			_plugin = plugin;


			PropertyInfo[] properties = SetterPropertyAttribute.FindMarkedProperties(plugin.PluggedType);
			foreach (PropertyInfo property in properties)
			{
				addSetterProperty(property, property.Name);
			}
		}

		public SetterProperty Add(string propertyName)
		{
			PropertyInfo property = _plugin.PluggedType.GetProperty(propertyName);
			addSetterProperty(property, propertyName);

			return (SetterProperty) _setterProperties[propertyName];
		}

		private void addSetterProperty(PropertyInfo property, string propertyName)
		{
			if (property == null)
			{
				throw new StructureMapException(240, propertyName, _plugin.PluggedType);
			}

			if (property.GetSetMethod() == null)
			{
				throw new StructureMapException(241, propertyName, _plugin.PluggedType);
			}

			SetterProperty setterProperty = new SetterProperty(property);
			_setterProperties.Add(propertyName, setterProperty);
		}

		protected override ICollection innerCollection
		{
			get { return _setterProperties.Values; }
		}

		public bool Contains(string propertyName)
		{
			return _setterProperties.Contains(propertyName);
		}

		public SetterProperty[] Setters
		{
			get
			{
				SetterProperty[] returnValue = new SetterProperty[_setterProperties.Count];
				_setterProperties.Values.CopyTo(returnValue, 0);

				return returnValue;
			}
		}


	}
}