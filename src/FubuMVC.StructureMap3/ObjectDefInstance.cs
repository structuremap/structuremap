using System.Linq;
using FubuMVC.Core.Registration.ObjectGraph;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap3
{
    public class ObjectDefInstance : ConfiguredInstance, IDependencyVisitor
    {
        public ObjectDefInstance(ObjectDef definition)
            : base(definition.Type)
        {
            definition.AcceptVisitor(this);
            Name = definition.Name;
        }

        void IDependencyVisitor.Value(ValueDependency dependency)
        {
            Dependencies.Add(dependency.DependencyType, dependency.Value);
        }

        void IDependencyVisitor.Configured(ConfiguredDependency dependency)
        {
            if (dependency.Definition.Value != null)
            {
                Dependencies.Add(dependency.DependencyType, dependency.Definition.Value);
            }
            else
            {
                var child = new ObjectDefInstance(dependency.Definition);
                Dependencies.Add(dependency.DependencyType, child);
            }
        }

        void IDependencyVisitor.List(ListDependency dependency)
        {
            var elements = dependency.Items.Select(instanceFor).ToArray();
            var instance = new EnumerableInstance(elements);

            Dependencies.Add(dependency.DependencyType, instance);
        }

        private Instance instanceFor(ObjectDef def)
        {
            return def.Value != null 
                ? (Instance) new ObjectInstance(def.Value) 
                : new ObjectDefInstance(def);
        }
    }
}