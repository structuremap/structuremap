using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    public class BuildPlanVisualizer : IBuildPlanVisitor
    {
        private readonly IPipelineGraph _pipeline;
        private readonly TextTreeWriter _writer;
        private readonly int _maxLevels;

        public BuildPlanVisualizer(IPipelineGraph pipeline, bool deep = false, int levels = 0)
        {
            _pipeline = pipeline;
            _writer = new TextTreeWriter();

            _maxLevels = deep ? int.MaxValue : levels;
        }

        public int MaxLevels
        {
            get { return _maxLevels; }
        }

        public void Constructor(ConstructorStep constructor)
        {
            _writer.Line(constructor.Description);
            if (constructor.Arguments.Any())
            {
                _writer.StartSection<OutlineWithBars>();

                constructor.Arguments.Each(arg => {
                    var depVisualizer = new DependencyVisualizer(arg.Title, _writer, this);
                    arg.Dependency.AcceptVisitor(depVisualizer);
                });

                _writer.EndSection();
            }

        }


        public void Setter(Setter setter)
        {
            var depVisualizer = new DependencyVisualizer(setter.Title, _writer, this);
            setter.AssignedValue.AcceptVisitor(depVisualizer);
        }

        public void Activator(IInterceptor interceptor)
        {
            _writer.Line(interceptor.Description);
        }

        public void Decorator(IInterceptor interceptor)
        {
            _writer.Line("Decorator --> " + interceptor.Description);
        }

        private readonly Stack<Instance> _instanceStack = new Stack<Instance>(); 

        public void Instance(Type pluginType, Instance instance)
        {
            if (_instanceStack.Any() && _instanceStack.Peek() == instance) return;

            if (_instanceStack.Contains(instance))
            {
                _writer.Line("Bi-Directional Relationship Detected w/ Instance {0}, PluginType {1}", instance.Description, pluginType.GetTypeName());
            }

            _instanceStack.Push(instance);


            _writer.Line("Build Plan for Instance {0}", instance.Description, instance.Name);
            if (pluginType != null) _writer.Line("PluginType: " + pluginType.GetFullName());
            _writer.Line("Lifecycle: " + (instance.Lifecycle ?? Lifecycles.Transient).Description);

            var plan = instance.ResolveBuildPlan(pluginType ?? instance.ReturnedType, _pipeline.Policies);

            plan.AcceptVisitor(this);
            _instanceStack.Pop();
        }

        public void InnerBuilder(IDependencySource inner)
        {
            _writer.Line(inner.Description);
        }

        public void Write(TextWriter writer)
        {
            _writer.WriteAll(writer);
        }
    }


    public class NestedSectionPadding : ILeftPadding
    {
        private readonly LeftPadding _outer;
        private readonly ILeftPadding _inner;

        public NestedSectionPadding(ILeftPadding inner, int spaces)
        {
            _outer = new LeftPadding(spaces, Convert.ToChar(9475).ToString());
            _inner = inner;
        }

        public string Create()
        {
            return _outer.Create().Replace(Convert.ToChar(9507), Convert.ToChar(9475)) + _inner.Create();
        }
    }
}