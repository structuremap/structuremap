using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace StructureMap.LegacyAttributeSupport
{
    public class FamilyAttributeScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            throw new NotImplementedException("do something else");
        }

        public Registry ScanTypes(TypeSet types)
        {
            throw new NotImplementedException();
        }
    }
}