using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Testing.Samples
{
    public interface IVisualizer<T>
    {
        string Visualize(T target);
    }

    public class BasicVisualizer<T> : IVisualizer<T>
    {
        public string Visualize(T target)
        {
            return target.ToString();
        }
    }

    public class SpecialLog { }

    public class SpecialLogVisualizer : IVisualizer<SpecialLog>
    {
        public string Visualize(SpecialLog target)
        {
            return "Special HTML Generation";
        }
    }

    public class GenericSamples
    {
        public void BuildContainer()
        {
            var container = new Container(_ =>
            {
                _.For(typeof (IVisualizer<>)).Use(typeof (BasicVisualizer<>));

                _.For<IVisualizer<SpecialLog>>().Use<SpecialLogVisualizer>();
                // Or conventionally -->
                _.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.ConnectImplementationsToTypesClosing(typeof (IVisualizer<>));
                });
            });

            container.GetInstance<IVisualizer<SpecialLog>>()
                .ShouldBeOfType<SpecialLogVisualizer>();
        }
    }

    // SAMPLE: nhibernate-isession-factory
    public interface ISession { }

    public interface ISessionFactory
    {
        ISession Build();
    }
    // ENDSAMPLE

    // SAMPLE: SessionFactoryRegistry
    public class SessionFactoryRegistry : Registry
    {
        // Let's not worry about how ISessionFactory is built
        // in this example
        public SessionFactoryRegistry(ISessionFactory factory)
        {
            For<ISessionFactory>().Use(factory);


            For<ISession>().Use("Build ISession from ISessionFactory", c =>
            {
                // To resolve ISession, I first pull out
                // ISessionFactory from the IContext and use that
                // to build a new ISession. 
                return c.GetInstance<ISessionFactory>().Build();
            });
        }
    }
    // ENDSAMPLE



    public class SessionFactory : ISessionFactory
    {
        public ISession Build()
        {
            throw new System.NotImplementedException();
        }
    }
}