using System;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;

namespace StructureMap
{
	/// <summary>
	/// GoF Memento representing an Object Instance
	/// </summary>
	public abstract class InstanceMemento
	{
		public const string EMPTY_STRING = "STRING.EMPTY";
		public const string TEMPLATE_ATTRIBUTE = "Template";
		public const string SUBSTITUTIONS_ATTRIBUTE = "Substitutions";
		private string _lastKey = string.Empty;
		private DefinitionSource _definitionSource = DefinitionSource.Explicit;

		/// <summary>
		/// The named type of the object instance represented by the InstanceMemento.  Translates to a concrete
		/// type
		/// </summary>
		public abstract string ConcreteKey { get; }

		/// <summary>
		/// The named key of the object instance represented by the InstanceMemento
		/// </summary>
		public abstract string InstanceKey { get; }

		/// <summary>
		/// Retrieves the named property value as a string
		/// </summary>
		/// <param name="Key"></param>
		/// <returns></returns>
		public string GetProperty(string Key)
		{
			string returnValue = "";

			try
			{
				returnValue = this.getPropertyValue(Key);
			}
			catch (Exception ex)
			{
				throw new StructureMapException(205, ex, Key, this.InstanceKey);
			}

			if (returnValue == string.Empty || returnValue == null)
			{
				throw new StructureMapException(205, Key, this.InstanceKey);
			}

			if (returnValue.ToUpper() == EMPTY_STRING)
			{
				returnValue = string.Empty;
			}

			return returnValue;
		}

		/// <summary>
		/// Gets the referred template name
		/// </summary>
		/// <returns></returns>
		public string TemplateName
		{
			get
			{
				string rawValue = getPropertyValue(TEMPLATE_ATTRIBUTE);
				return rawValue == null ? string.Empty : rawValue.Trim();
			}
		}

		/// <summary>
		/// Template method for implementation specific retrieval of the named property
		/// </summary>
		/// <param name="Key"></param>
		/// <returns></returns>
		protected abstract string getPropertyValue(string Key);

		/// <summary>
		/// Returns the last key/value retrieved for exception tracing 
		/// </summary>
		public string LastKey
		{
			get { return _lastKey; }
		}

		/// <summary>
		/// Returns the named child InstanceMemento
		/// </summary>
		/// <param name="Key"></param>
		/// <returns></returns>
		public InstanceMemento GetChildMemento(string Key)
		{
			_lastKey = Key;

			InstanceMemento returnValue = this.getChild(Key);
			return returnValue;
		}

		/// <summary>
		/// Template method for implementation specific retrieval of the named property
		/// </summary>
		/// <param name="Key"></param>
		/// <returns></returns>
		protected abstract InstanceMemento getChild(string Key);

		/// <summary>
		/// Using InstanceManager and the TypeName, creates an object instance using the
		/// child InstanceMemento specified by Key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="typeName"></param>
		/// <param name="manager"></param>
		/// <returns></returns>
		public object GetChild(string key, string typeName, InstanceManager manager)
		{
			InstanceMemento memento = this.GetChildMemento(key);
			object returnValue = null;

			if (memento == null)
			{
				try
				{
					returnValue = manager.CreateInstance(typeName);
				}
				catch (Exception ex)
				{
					throw new StructureMapException(209, ex, key, typeName);
				}
			}
			else
			{
				returnValue = manager.CreateInstance(typeName, memento);
			}


			return returnValue;
		}


		/// <summary>
		/// Not used yet.
		/// </summary>
		/// <param name="Key"></param>
		/// <returns></returns>
		public string[] GetStringArray(string Key)
		{
			string _value = this.GetProperty(Key);
			return _value.Split(new char[] {','});
		}

		/// <summary>
		/// This method is made public for testing.  It is not necessary for normal usage.
		/// </summary>
		/// <returns></returns>
		public abstract InstanceMemento[] GetChildrenArray(string Key);


		/// <summary>
		/// Template pattern property specifying whether the InstanceMemento is simply a reference
		/// to another named instance.  Useful for child objects.
		/// </summary>
		public abstract bool IsReference { get; }

		/// <summary>
		/// Template pattern property specifying the instance key that the InstanceMemento refers to
		/// </summary>
		public abstract string ReferenceKey { get; }


		/// <summary>
		/// Is the InstanceMemento a reference to the default instance of the plugin type?
		/// </summary>
		public bool IsDefault
		{
			get { return (IsReference && ReferenceKey == string.Empty); }
		}

		/// <summary>
		/// Used to create a templated InstanceMemento
		/// </summary>
		/// <param name="memento"></param>
		/// <returns></returns>
		public virtual InstanceMemento Substitute(InstanceMemento memento)
		{
			throw new NotSupportedException("This type of InstanceMemento does not support the Substitute() Method");
		}


		public virtual TemplateToken CreateTemplateToken()
		{
			throw new NotSupportedException("This type of InstanceMemento does not support the CreateTemplateToken() Method");
		}

		public DefinitionSource DefinitionSource
		{
			get { return _definitionSource; }
			set { _definitionSource = value; }
		}
	}
}