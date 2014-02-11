using System;
using System.IO;
using System.Reflection;
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
        private bool _is_not_the_first_Instance;
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

        public void Constructor(ConstructorInfo constructor)
        {
            throw new NotImplementedException();
        }

        public void Parameter(ParameterInfo parameter, IDependencySource source)
        {
            throw new NotImplementedException();
        }

        public void Setter(Setter setter)
        {
            throw new NotImplementedException();
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
            if (_is_not_the_first_Instance)
            {
                _writer.BlankLines(3);
            }
            _is_not_the_first_Instance = true;

            var title = "Build Plan for Instance {0}";
            if (instance.HasExplicitName())
            {
                title += " ('{1}')";
            }

            _writer.Line(title, instance.Description, instance.Name);
            if (pluginType != null) _writer.Line("PluginType: " + pluginType.GetFullName());
            _writer.Line("Lifecycle: " + instance.Lifecycle.Description);

            var plan = instance.ResolveBuildPlan(pluginType, _pipeline.Policies);

            plan.AcceptVisitor(this);

        }

        public void InnerBuilder(IDependencySource inner)
        {
            throw new NotImplementedException();
        }

        public void Write(TextWriter writer)
        {
            _writer.WriteAll(writer);
        }
    }
}