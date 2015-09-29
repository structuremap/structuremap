using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace StructureMap.LegacyAttributeSupport
{
    public class PluggableAttributeScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {

            throw new NotImplementedException("do it a different way");
//            if (PluggableAttribute.MarkedAsPluggable(type))
//            {
//                //registry.AddType(type);
//            }
        }

        public Registry ScanTypes(TypeSet types)
        {
            throw new NotImplementedException();
        }
    }
}