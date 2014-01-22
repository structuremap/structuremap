using System;

namespace StructureMap.Graph
{
    /// <summary>
    /// Designates a CLR type that is loaded by name.
    /// </summary>
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


        public Type FindType()
        {
            try
            {
                return Type.GetType(AssemblyQualifiedName, true);
            }
            catch (Exception e)
            {
                var message = string.Format("Could not create a Type for '{0}'", AssemblyQualifiedName);
                throw new ArgumentException(message, e);
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