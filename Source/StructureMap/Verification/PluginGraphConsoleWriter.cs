using System;
using System.IO;
using System.Text;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Verification
{
    /// <summary>
    /// Creates a textual report summarizing the StructureMap configuration of an application,
    /// including any implicitly defined PluginFamily's and Plugin's marked by attributes
    /// </summary>
    public class PluginGraphConsoleWriter : MarshalByRefObject
    {
        private readonly PluginGraphReport _report;
        private TextWriter _writer;
        private bool _includePlugins = false;
        private bool _includeAllInstances = false;
        private bool _includeSource = false;
        private bool _writeAll = false;
        private bool _writeProblems = false;

        public PluginGraphConsoleWriter(PluginGraphReport report) : base()
        {
            _report = report;
        }

        public void Write(TextWriter writer)
        {
            _writer = writer;

            writeAssemblies();
            _writer.WriteLine(_writer.NewLine);

            writePluginFamilies();

            if (WriteProblems || WriteAll)
            {
                writeProblems();
            }
        }


        private void writeSeparator()
        {
            _writer.WriteLine(
                "----------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        private void writeAssemblies()
        {
            _writer.WriteLine("Assemblies");
            foreach (AssemblyToken assembly in _report.Assemblies)
            {
                _writer.WriteLine(assembly.ToString());
            }
            writeSeparator();
        }


        private void writePluginFamilies()
        {
            _writer.WriteLine("PluginFamilies");
            foreach (FamilyToken family in _report.Families)
            {
                writePluginFamily(family);
            }
        }

        private void writePluginFamily(FamilyToken family)
        {
            writeSeparator();

            _writer.WriteLine(family.ToString());


            if (_includeSource || _writeAll)
            {
                writeSource(family);
            }

            if (_includePlugins || _writeAll)
            {
                writePlugins(family);
            }

            if (_includeAllInstances || _writeAll)
            {
                writeInstances(family);
            }
        }

        private void writeSource(FamilyToken family)
        {
            if (family.SourceInstance == null)
            {
                return;
            }

            _writer.WriteLine("    Source:  " + family.SourceInstance.ToString());
            _writer.WriteLine();
        }

        private void writeInstances(FamilyToken family)
        {
            foreach (InstanceToken instance in family.Instances)
            {
                _writer.WriteLine("    Instance:  " + instance.ToString());
            }
            _writer.WriteLine();
        }

        private void writePlugins(FamilyToken family)
        {
            foreach (PluginToken plugin in family.Plugins)
            {
                _writer.WriteLine("    " + plugin.ToString());
            }
            _writer.WriteLine();
        }

        public string GetReport()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(builder);

            Write(stringWriter);

            return builder.ToString();
        }

        private void writeProblems()
        {
            writeSeparator();
            _writer.WriteLine("Problems:");
            ProblemFinder problemFinder = new ProblemFinder(_report);

            Problem[] problems = problemFinder.GetProblems();
            foreach (Problem problem in problems)
            {
                _writer.WriteLine(problem.Path);
                _writer.WriteLine(problem.ToString());
                writeSeparator();
            }
        }

        #region properties

        public bool IncludePlugins
        {
            get { return _includePlugins; }
            set { _includePlugins = value; }
        }

        public bool IncludeAllInstances
        {
            get { return _includeAllInstances; }
            set { _includeAllInstances = value; }
        }

        public bool IncludeSource
        {
            get { return _includeSource; }
            set { _includeSource = value; }
        }

        public bool WriteAll
        {
            get { return _writeAll; }
            set { _writeAll = value; }
        }

        public bool WriteProblems
        {
            get { return _writeProblems; }
            set { _writeProblems = value; }
        }

        #endregion
    }
}