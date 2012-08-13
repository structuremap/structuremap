using System;
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
        public TypePath(string assemblyName, string className)
        {
            AssemblyQualifiedName = className + "," + assemblyName;
        }

        public TypePath(Type type)
        {
            AssemblyQualifiedName = type.AssemblyQualifiedName;
        }

        public TypePath(string assemblyQualifiedName)
        {
            AssemblyQualifiedName = assemblyQualifiedName;
        }

        public string AssemblyQualifiedName { get; private set; }

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
            var peer = obj as TypePath;
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