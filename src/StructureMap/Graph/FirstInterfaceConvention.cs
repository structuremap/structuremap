using System;
using System.Diagnostics;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class FirstInterfaceConvention : ConfigurableRegistrationConvention
    {
        public override void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete() || !type.CanBeCreated()) return;


            var interfaceType = type.AllInterfaces().FirstOrDefault();
            if (interfaceType != null)
            {
                Debug.WriteLine("Plugging {0} into {1}".ToFormat(type.Name, interfaceType.Name));
                registry.AddType(interfaceType, type);
                ConfigureFamily(registry.For(interfaceType));
            }
        }

        public override void ScanTypes(TypeSet types, Registry registry)
        {
            types.FindTypes(TypeClassification.Concretes).Where(x => x.HasConstructors()).Each(type =>
            {
                var interfaceType = type.AllInterfaces().FirstOrDefault();
                if (interfaceType != null)
                {
                    registry.AddType(interfaceType, type);
                    ConfigureFamily(registry.For(interfaceType));
                }
            });

        }
    }
}