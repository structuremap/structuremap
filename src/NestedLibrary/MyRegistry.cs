using System;
using System.Linq;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace NestedLibrary
{
    public class MyRegistry : Registry
    {
        public MyRegistry()
        {
            Scan(
                x =>
                {
                    x.TheCallingAssembly();
                    x.Convention<MyConvention>();
                });
        }
    }

    public class MyConvention : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (type.CanBeCastTo<ITeam>() && type != typeof(ITeam))
            {
                registry.For(typeof(ITeam)).Add(type);
            }
        }

        public Registry ScanTypes(TypeSet types)
        {
            var registry = new Registry();

            var matches = types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)
                .Where(type => type.CanBeCastTo<ITeam>());

            foreach (var type in matches)
            {
                registry.For(typeof (ITeam)).Add(type);
            }

            return registry;
        }
    }

    public interface ITeam{}

    public class Chiefs : ITeam{}
    public class Chargers : ITeam{}
    public class Broncos : ITeam{}
    public class Raiders : ITeam{}
}