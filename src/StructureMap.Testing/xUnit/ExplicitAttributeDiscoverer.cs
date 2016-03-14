using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace StructureMap.Testing.xUnit
{
    public class ExplicitAttributeDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>("Explicit", "true");
        }
    }
}