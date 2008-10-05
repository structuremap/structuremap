using System;
using System.IO;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class ValidationError
    {
        public Exception Exception;
        public Instance Instance;
        public string MethodName;
        public Type PluginType;

        public ValidationError(Type pluginType, Instance instance, Exception exception, MethodInfo method)
        {
            PluginType = pluginType;
            Instance = instance;
            Exception = exception;
            MethodName = method.Name;
        }

        public void Write(StringWriter writer)
        {
            string description = ((IDiagnosticInstance) Instance).CreateToken().Description;

            writer.WriteLine();
            writer.WriteLine(
                "-----------------------------------------------------------------------------------------------------");
            writer.WriteLine("Validation Error in Method {0} of Instance '{1}' ({2})\n   in PluginType {3}", MethodName,
                             Instance.Name, description, TypePath.GetAssemblyQualifiedName(PluginType));
            writer.WriteLine();
            writer.WriteLine(Exception.ToString());
            writer.WriteLine();
        }
    }
}