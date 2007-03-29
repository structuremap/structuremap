using System;
using System.IO;
using StructureMap.Configuration;

namespace StructureMap.Verification
{
    public class StartUp : IStartUp
    {
        private string _problemFile = "StructureMap.error";
        private bool _fail = false;
        private string _allFile;

        public IStartUp WriteProblemsTo(string fileName)
        {
            _problemFile = fileName;
            return this;
        }

        public IStartUp FailOnException()
        {
            _fail = true;
            return this;
        }

        public IStartUp WriteAllTo(string fileName)
        {
            _allFile = fileName;
            return this;
        }

        public void RunDiagnostics()
        {
            PluginGraphReport report = StructureMapConfiguration.GetDiagnosticReport();

            writeProblems(report);

            writeAll(report);

            fail(report);
        }

        private void fail(PluginGraphReport report)
        {
            if (_fail)
            {
                ProblemFinder finder = new ProblemFinder(report);
                Problem[] problems = finder.GetProblems();

                if (problems.Length > 0)
                {
                    throw new ApplicationException(
                        "StructureMap detected configuration or environmental problems.  Check the StructureMap error file");
                }
            }
        }

        private void writeAll(PluginGraphReport report)
        {
            if (!string.IsNullOrEmpty(_allFile))
            {
                PluginGraphConsoleWriter consoleWriter = new PluginGraphConsoleWriter(report);
                consoleWriter.WriteAll = true;
                consoleWriter.WriteProblems = true;
                
                using (TextWriter writer = new StreamWriter(_allFile))
                {
                    consoleWriter.Write(writer);
                }
            }
        }

        private void writeProblems(PluginGraphReport report)
        {
            using (TextWriter writer = new StreamWriter(_problemFile))
            {
                PluginGraphConsoleWriter consoleWriter = new PluginGraphConsoleWriter(report);
                consoleWriter.IncludeAllInstances = false;
                consoleWriter.IncludePlugins = false;
                consoleWriter.IncludeSource = false;
                consoleWriter.WriteProblems = true;
                consoleWriter.Write(writer);
            }
        }
    }
}