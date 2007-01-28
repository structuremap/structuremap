using System.Collections;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
    public class ProblemFinder : IConfigurationVisitor
    {
        public static Problem[] FindProblems(string configPath)
        {
            RemoteGraphContainer container = new RemoteGraphContainer(configPath);
            RemoteGraph remoteGraph = container.GetRemoteGraph();
            PluginGraphReport report = remoteGraph.GetReport();
            ProblemFinder finder = new ProblemFinder(report);

            return finder.GetProblems();
        }

        public static Problem[] FindProblems(string configPath, string binPath)
        {
            if (binPath == string.Empty || binPath == null)
            {
                return FindProblems(configPath);
            }

            RemoteGraphContainer container = new RemoteGraphContainer(configPath, binPath);
            RemoteGraph remoteGraph = container.GetRemoteGraph();
            PluginGraphReport report = remoteGraph.GetReport();
            ProblemFinder finder = new ProblemFinder(report);

            return finder.GetProblems();
        }

        public static Problem[] FindProblems(PluginGraphReport report)
        {
            ProblemFinder finder = new ProblemFinder(report);
            return finder.GetProblems();
        }

        private readonly PluginGraphReport _report;
        private ArrayList _problems = new ArrayList();
        private Stack _graphPath = new Stack();

        public ProblemFinder(PluginGraphReport report)
        {
            _report = report;
        }

        public Problem[] GetProblems()
        {
            GraphObjectIterator iterator = new GraphObjectIterator(this);
            iterator.Visit(_report);
            return (Problem[]) _problems.ToArray(typeof (Problem));
        }

        public void StartObject(GraphObject node)
        {
            _graphPath.Push(node.ToString());

            Problem[] problems = node.Problems;
            if (problems.Length > 0)
            {
                string path = buildPathString();
                foreach (Problem problem in problems)
                {
                    problem.Path = path;
                    problem.ObjectId = node.Id;
                }

                _problems.AddRange(problems);
            }
        }

        public void EndObject(GraphObject node)
        {
            _graphPath.Pop();
        }

        private string buildPathString()
        {
            string pad = "";
            string path = "";

            foreach (object node in _graphPath)
            {
                path += pad + node + "\n";
                pad += "     ";
            }

            return path;
        }

        public void HandleAssembly(AssemblyToken assembly)
        {
            // no-op
        }

        public void HandleFamily(FamilyToken family)
        {
            // no-op
        }

        public void HandleMementoSource(MementoSourceInstanceToken source)
        {
            // no-op
        }

        public void HandlePlugin(PluginToken plugin)
        {
            // no-op
        }

        public void HandleInterceptor(InterceptorInstanceToken interceptor)
        {
            // no-op
        }

        public void HandleInstance(InstanceToken instance)
        {
            // no-op
        }

        public void HandlePrimitiveProperty(PrimitiveProperty property)
        {
            // no-op
        }

        public void HandleEnumerationProperty(EnumerationProperty property)
        {
            // no-op
        }

        public void HandleInlineChildProperty(ChildProperty property)
        {
            // no-op
        }

        public void HandleDefaultChildProperty(ChildProperty property)
        {
            // no-op
        }

        public void HandleReferenceChildProperty(ChildProperty property)
        {
            // no-op
        }

        public void HandlePropertyDefinition(PropertyDefinition propertyDefinition)
        {
            // no-op
        }

        public void HandleChildArrayProperty(ChildArrayProperty property)
        {
            // no-op
        }

        public void HandleNotDefinedChildProperty(ChildProperty property)
        {
            // no-op
        }

        public void HandleTemplate(TemplateToken template)
        {
            // no-op
        }

        public void HandleTemplateProperty(TemplateProperty property)
        {
            // no-op
        }
    }
}