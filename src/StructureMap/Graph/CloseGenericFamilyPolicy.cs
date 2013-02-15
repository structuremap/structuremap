using System;

namespace StructureMap.Graph
{
    public class CloseGenericFamilyPolicy : IFamilyPolicy
    {
        private readonly PluginGraph _graph;

        public CloseGenericFamilyPolicy(PluginGraph graph)
        {
            _graph = graph;
        }

        public bool Matches(Type type)
        {
            if (!type.IsGenericType) return false;

            var basicType = type.GetGenericTypeDefinition();
            return _graph.Families.Has(basicType);
        }

        public PluginFamily Build(Type type)
        {
            if (!type.IsGenericType) return null;

            var basicType = type.GetGenericTypeDefinition();
            if (!_graph.Families.Has(basicType))
            {
                return null;
            }

            var basicFamily = _graph.Families[basicType];
            Type[] templatedParameterTypes = type.GetGenericArguments();


            PluginFamily templatedFamily = basicFamily.CreateTemplatedClone(templatedParameterTypes);
            PluginFamily family = templatedFamily;
            _graph.ProfileManager.CopyDefaults(basicType, type, family);

            return family;
        }
    }
}