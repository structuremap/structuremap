using System;
using StructureMap.Building;
using StructureMap.Diagnostics.TreeView;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    // TODO -- add level latching?
    public class DependencyVisualizer : IDependencyVisitor, IDisposable
    {
        private readonly TitledWriter _writer;
        private readonly BuildPlanVisualizer _buildPlanVisitor;

        public DependencyVisualizer(string title, TreeWriter writer, BuildPlanVisualizer buildPlanVisitor)
        {
            _writer = new TitledWriter(title, writer);
            _buildPlanVisitor = buildPlanVisitor;
        }

        public void Constant(Constant constant)
        {
            Dependency(constant);
        }

        public void Default(Type pluginType)
        {
            _writer.Line("**Default**");
            _buildPlanVisitor.ShowDefault(pluginType);
        }

        public void Referenced(ReferencedDependencySource source)
        {
            _writer.Line("Instance named '{0}'".ToFormat(source.Name));
            _buildPlanVisitor.ShowReferenced(source.DependencyType, source.Name);
        }

        public void InlineEnumerable(IEnumerableDependencySource source)
        {
            _writer.Line("Inline Enumerable Configuration");

            int i = 0;

            source.Items.Each(x => {
                i++;
                var title = (i.ToString() + ".) ").PadLeft(5);
                using (var child = new DependencyVisualizer(title, _writer.Writer, _buildPlanVisitor))
                {
                    x.AcceptVisitor(child);
                    _writer.Line("");
                }
            });

        }

        public void AllPossibleOf(Type pluginType)
        {
            _writer.Line("All registered Instances of " + pluginType.GetFullName());
        }

        public void Concrete(ConcreteBuild build)
        {
            _writer.Line(build.Description);
        }

        public void Lifecycled(LifecycleDependencySource source)
        {
            _writer.Line(source.Description);
            _buildPlanVisitor.Instance(null, source.Instance);
        }

        public void Dependency(IDependencySource source)
        {
            _writer.Line(source.Description);
        }

        public void Problem(DependencyProblem problem)
        {
            _writer.Line(problem.Message);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}