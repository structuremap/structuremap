using System;
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
            throw new NotImplementedException();
        }
    }

    public interface ITeam{}

    public class Chiefs : ITeam{}
    public class Chargers : ITeam{}
    public class Broncos : ITeam{}
    public class Raiders : ITeam{}
}