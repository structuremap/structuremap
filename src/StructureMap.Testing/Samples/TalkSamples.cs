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


    public interface ISession { }

    public interface ISessionFactory
    {
        ISession Build();
    }

    public class LambdaRegistry : Registry
    {
        public LambdaRegistry()
        {
            For<ISession>().Use(c => c.GetInstance<ISessionFactory>().Build());
        }
    }
}