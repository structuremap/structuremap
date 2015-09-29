using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace StructureMap.LegacyAttributeSupport
{
    public class PluggableAttributeScanner : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            throw new NotImplementedException();
        }
    }
}