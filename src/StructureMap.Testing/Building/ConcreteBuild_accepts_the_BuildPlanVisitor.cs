using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class ConcreteBuild_accepts_the_BuildPlanVisitor
    {
        [Fact]
        public void sends_the_constructor_args_and_parameters_to_the_visitor()
        {
            var dependencies = new DependencyCollection();
            dependencies.Add(typeof(Rule), new ColorRule("Red"));
            dependencies.Add(typeof(IWidget), new AWidget());

            var build = ConcreteType.BuildSource(typeof(GuyWithCtorAndArgs), null, dependencies,
                Policies.Default());

            var visitor = new StubVisitor();
            build.AcceptVisitor(visitor);

            visitor.Items.ShouldHaveTheSameElementsAs(
                "Constructor: Void .ctor(StructureMap.Testing.Widget3.IGateway, StructureMap.Testing.Widget.Rule)",
                "Set IWidget Widget = Value: StructureMap.Testing.Widget.AWidget",
                "Set IService Service = *Default of IService*"
                );
        }
    }

    public class StubVisitor : IBuildPlanVisitor
    {
        public readonly IList<string> Items = new List<string>();

        public void Constructor(ConstructorStep constructor)
        {
            Items.Add("Constructor: " + constructor.Constructor);
        }

        public void Parameter(ParameterInfo parameter, IDependencySource source)
        {
            Items.Add("Parameter: " + parameter.Name + ", " + source.Description);
        }

        public void Setter(Setter setter)
        {
            Items.Add(setter.Description);
        }

        public void Activator(IInterceptor interceptor)
        {
            throw new NotImplementedException();
        }

        public void Decorator(IInterceptor interceptor)
        {
            throw new NotImplementedException();
        }

        public void Instance(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public void InnerBuilder(IDependencySource inner)
        {
            throw new NotImplementedException();
        }
    }
}