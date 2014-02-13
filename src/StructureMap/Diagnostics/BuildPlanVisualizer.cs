using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Diagnostics.TreeView;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    public class BuildPlanVisualizer : IBuildPlanVisitor
    {
        private readonly IPipelineGraph _pipeline;
        private readonly TreeWriter _writer;
        private readonly int _maxLevels;
        private int _level;

        public BuildPlanVisualizer(IPipelineGraph pipeline, bool deep = false, int levels = 0)
        {
            _pipeline = pipeline;
            _writer = new TreeWriter();

            _maxLevels = deep ? int.MaxValue : levels;
        }

        public int MaxLevels
        {
            get { return _maxLevels; }
        }

        public void VisitNextLevel(Action action)
        {
            _writer.StartSection(new LeftBorder(3));
            if (_level >= _maxLevels) return;

            _level++;

            try
            {
                action();
            }
            finally
            {
                _level--;
                _writer.EndSection();
                _writer.Line("");
            }
        }

        public void ShowDefault(Type pluginType)
        {
            VisitNextLevel(() => {
                var @default = _pipeline.Instances.GetDefault(pluginType);
                if (@default == null)
                {
                    _writer.Line("NO DEFAULT IS SPECIFIED AND CANNOT BE AUTOMATICALLY DETERMINED");
                }
                else
                {
                    Instance(pluginType, @default);
                }
            });
        }

        public void ShowReferenced(Type pluginType, string name)
        {
            VisitNextLevel(() => {
                var instance = _pipeline.Instances.FindInstance(pluginType, name);
                if (instance == null)
                {
                    _writer.Line("NO SUCH INSTANCE IS REGISTERED FOR THIS NAME");
                }
                else
                {
                    Instance(pluginType, instance);
                }
            });
        }

        public void Constructor(ConstructorStep constructor)
        {
            _writer.Line(constructor.Description);
            if (constructor.Arguments.Any())
            {
                _writer.StartSection<Outline>();

                constructor.Arguments.Each(arg => {
                    using (var depVisualizer = new DependencyVisualizer(arg.Title, _writer, this))
                    {
                        arg.Dependency.AcceptVisitor(depVisualizer);
                    }
                });

                _writer.EndSection();
            }

        }


        public void Setter(Setter setter)
        {
            using (var depVisualizer = new DependencyVisualizer(setter.Title, _writer, this))
            {
                setter.AssignedValue.AcceptVisitor(depVisualizer);
            }
        }

        public void Activator(IInterceptor interceptor)
        {
            _writer.Line(interceptor.Description);
        }

        public void Decorator(IInterceptor interceptor)
        {
            _writer.Line("Decorator --> " + interceptor.Description);
            var decorator = interceptor as DecoratorInterceptor;
            if (decorator != null)
            {
                _writer.StartSection(4);

                var build = decorator.ToConcreteBuild(_pipeline.Policies, null);
                build.AcceptVisitor(this);
                _writer.EndSection();
            }
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



}