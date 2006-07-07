using System;
using System.Reflection;
using System.Xml;
using StructureMap.Configuration;

namespace StructureMap.Graph
{
	/// <summary>
	/// Designates a CLR type that is loaded by name.
	/// </summary>
	public class TypePath
	{
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
		private string _assemblyName;

		public string ClassName
		{
			get { return _className; }
		}

		public string AssemblyName
		{
			get { return _assemblyName; }
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
			Assembly assembly = AppDomain.CurrentDomain.Load(_assemblyName);
			return assembly.GetType(_className, true);
		}

		public override bool Equals(object obj)
		{
			TypePath peer = obj as TypePath;
			if (peer == null)
			{
				return false;
			}

			return peer.FindType().Equals(this.FindType());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
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
	}
}