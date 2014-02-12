using System;
using StructureMap.Building;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    // TODO -- add level latching?
    public class DependencyVisualizer : IDependencyVisitor
    {
        private readonly string _title;
        private readonly TextTreeWriter _writer;
        private readonly IBuildPlanVisitor _buildPlanVisitor;

        public DependencyVisualizer(string title, TextTreeWriter writer, IBuildPlanVisitor buildPlanVisitor)
        {
            _title = title;
            _writer = writer;
            _buildPlanVisitor = buildPlanVisitor;
        }

        private void write(string text)
        {
            _writer.Line(_title + text);
        }

        private void withinNestedSection(Action action)
        {
            var padding = new NestedSectionPadding(_writer.CurrentPadding, _title.Length);
            _writer.StartSection(new TreeSection(padding));
            action();
            _writer.EndSection();
        }

        public void Constant(Constant constant)
        {
            Dependency(constant);
        }

        public void Default(Type pluginType)
        {
            write("**Default**");
        }

        public void Referenced(ReferencedDependencySource source)
        {
            write("Instance named '{0}'".ToFormat(source.Name));
        }

        public void InlineEnumerable(IEnumerableDependencySource source)
        {
            write("Inline Enumerable Configuration");
            int i = 0;

            withinNestedSection(() => {
                

                source.Items.Each(x => {
                    i++;
                    var title = (i.ToString() + ".)").PadLeft(4);
                    var child = new DependencyVisualizer(title, _writer, _buildPlanVisitor);

                    x.AcceptVisitor(child);
                });
            });

        }

        public void AllPossibleOf(Type pluginType)
        {
            write("All registered Instances of " + pluginType.GetFullName());
        }

        public void Concrete(ConcreteBuild build)
        {
            write(build.Description);
        }

        public void Lifecycled(LifecycleDependencySource source)
        {
            write(source.Description);
            withinNestedSection(() => _buildPlanVisitor.Instance(null, source.Instance));
        }

        public void Dependency(IDependencySource source)
        {
            write(source.Description);
        }

        public void Problem(DependencyProblem problem)
        {
            write(problem.Message);
        }

    }
}