using System;
using System.Diagnostics;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;
using System.Linq;

namespace StructureMap.Graph
{
    public class FirstInterfaceConvention : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete() || !type.CanBeCreated()) return;
            
            
            var interfaceType = type.AllInterfaces().FirstOrDefault();
            if (interfaceType != null)
            {
                Debug.WriteLine("Plugging {0} into {1}".ToFormat(type.Name, interfaceType.Name));
                registry.AddType(interfaceType, type);
            }
        }
    }
}