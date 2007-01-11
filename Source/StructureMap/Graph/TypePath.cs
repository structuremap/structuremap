using System;
using System.Diagnostics;
using System.Xml;
using StructureMap.Configuration;

namespace StructureMap.Graph
{
	/// <summary>
	/// Designates a CLR type that is loaded by name.
	/// </summary>
	[Serializable]
	public class TypePath
	{
        public static string GetTypeIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        public static TypePath TypePathForFullName(string fullname)
        {
            return new TypePath(string.Empty, fullname);
        }

		public static TypePath CreateFromXmlNode(XmlNode node)
		{
			string typeName = node.Attributes[XmlConstants.TYPE_ATTRIBUTE].Value;
			string assemblyName = node.Attributes[XmlConstants.ASSEMBLY].Value;

			return new TypePath(assemblyName, typeName);
		}

		public static void WriteTypePathToXmlElement(Type type, XmlElement element)
		{
			element.SetAttribute(XmlConstants.TYPE_ATTRIBUTE, type.FullName);
			element.SetAttribute(XmlConstants.ASSEMBLY, type.Assembly.GetName().Name);
		}

		private string _className;
		private string _assemblyName = string.Empty;

		public string AssemblyQualifiedName
		{
			get { return _className + "," + _assemblyName; }
		}
	    
	    public static string GetAssemblyQualifiedName(Type type)
	    {
	        TypePath path = new TypePath(type);
            return path.AssemblyQualifiedName;
	    }

		public string AssemblyName
		{
			get { return _assemblyName; }
		}

	    public string ClassName
	    {
            get { return _className; }
	    }

	    public TypePath(string assemblyName, string className)
		{
			_className = className;
			_assemblyName = assemblyName;
		}

		public TypePath(Type type)
		{
			_className = type.FullName;
			_assemblyName = type.Assembly.GetName().Name;
		}

		public Type FindType()
		{
		    try
		    {
		        return Type.GetType(this.AssemblyQualifiedName, true);
		    }
		    catch (Exception e)
		    {
                string message = string.Format("Could not create a Type for '{0}'", this.AssemblyQualifiedName);
                throw new ApplicationException(message, e);
		    }
		}

        public bool Matches(string typeName)
        {
            return (this.AssemblyQualifiedName == typeName || this.ClassName == typeName) ;
        }

        public bool Matches(Type type)
        {
            return (this.AssemblyQualifiedName == type.AssemblyQualifiedName || this.ClassName == type.FullName);
        }

		public override bool Equals(object obj)
		{
			TypePath peer = obj as TypePath;
			if (peer == null)
			{
				return false;
			}

		    return peer.AssemblyQualifiedName == this.AssemblyQualifiedName;
		}

		public override int GetHashCode()
		{
		    return this.AssemblyQualifiedName.GetHashCode();
		}


		public bool CanFindType()
		{
			try
			{
				Type type = this.FindType();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

	    public override string ToString()
	    {
            return this.AssemblyQualifiedName;
	    }

	    public static TypePath GetTypePath(string name)
	    {
	        try
	        {
	            Type type = Type.GetType(name);
	            if (type != null)
	            {
	                return new TypePath(type);
	            }
	        }
	        catch (Exception ex)
	        {
	            Debug.WriteLine("here!");
	        }

            return TypePathForFullName(name);
	    }
	}
}