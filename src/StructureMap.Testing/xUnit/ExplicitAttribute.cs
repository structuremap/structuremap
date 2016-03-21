using System;
using Xunit.Sdk;

namespace StructureMap.Testing.xUnit
{
    [TraitDiscoverer("StructureMap.Testing.xUnit.ExplicitAttributeDiscoverer", "StructureMap.Testing")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExplicitAttribute : Attribute, ITraitAttribute
    {
        public ExplicitAttribute(string description)
        {
        }
    }
}