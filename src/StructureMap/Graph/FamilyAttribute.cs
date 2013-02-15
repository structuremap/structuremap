using System;

namespace StructureMap.Graph
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public abstract class FamilyAttribute : Attribute
    {
        public abstract void Alter(PluginFamily family);
    }
}