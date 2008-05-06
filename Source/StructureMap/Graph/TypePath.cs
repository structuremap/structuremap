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
        private string _assemblyName = string.Empty;
        private string _className;

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

        public TypePath(string assemblyQualifiedName)
        {
            string[] parts = assemblyQualifiedName.Split(',');
            if (parts.Length < 2)
            {
                throw new StructureMapException(107, assemblyQualifiedName);
            }

            _className = parts[0].Trim();
            _assemblyName = parts[1].Trim();
        }

        public string AssemblyQualifiedName
        {
            get { return _className + "," + _assemblyName; }
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        public string ClassName
        {
            get { return _className; }
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

        public static string GetAssemblyQualifiedName(Type type)
        {
            TypePath path = new TypePath(type);
            return path.AssemblyQualifiedName;
        }

        public Type FindType()
        {
            try
            {
                return Type.GetType(AssemblyQualifiedName, true);
            }
            catch (Exception e)
            {
                string message = string.Format("Could not create a Type for '{0}'", AssemblyQualifiedName);
                throw new ApplicationException(message, e);
            }
        }

        public override bool Equals(object obj)
        {
            TypePath peer = obj as TypePath;
            if (peer == null)
            {
                return false;
            }

            return peer.AssemblyQualifiedName == AssemblyQualifiedName;
        }

        public override int GetHashCode()
        {
            return AssemblyQualifiedName.GetHashCode();
        }

        public override string ToString()
        {
            return AssemblyQualifiedName;
        }
    }
}