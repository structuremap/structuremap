using System.Collections.Generic;
using System.IO;
using System.Text;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    public class WhatDoIHaveWriter
    {
        private readonly PipelineGraph _graph;
        private List<IInstance> _instances;
        private TextReportWriter _writer;

        public WhatDoIHaveWriter(PipelineGraph graph)
        {
            _graph = graph;
        }

        public string GetText()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            writeSources(writer);
            writeContentsOfPluginTypes(writer);

            return sb.ToString();
        }

        private void writeContentsOfPluginTypes(StringWriter stringWriter)
        {
            _writer = new TextReportWriter(3);
            _instances = new List<IInstance>();

            _writer.AddDivider('=');
            _writer.AddText("PluginType", "Name", "Description");

            foreach (PluginTypeConfiguration pluginType in _graph.PluginTypes)
            {
                writePluginType(pluginType);
            }

            _writer.AddDivider('=');

            _writer.Write(stringWriter);
        }

        private void writeSources(StringWriter writer)
        {
            writer.WriteLine(
                "===========================================================================================================");
            writer.WriteLine("Configuration Sources:");
            writer.WriteLine();

            for (int i = 0; i < _graph.Log.Sources.Length; i++)
            {
                string source = _graph.Log.Sources[i];
                string message = (i + ")").PadRight(5) + source;
                writer.WriteLine(message);
            }

            writer.WriteLine();
        }

        private void writePluginType(PluginTypeConfiguration pluginType)
        {
            _writer.AddDivider('-');
            var contents = new[]
                               {
                                   "{0} ({1})".ToFormat(pluginType.PluginType.GetName(), pluginType.PluginType.GetFullName()),
                                   string.Empty,
                                   string.Empty
                               };

            if (pluginType.Default != null)
            {
                setContents(contents, pluginType.Default);
            }

            _writer.AddText(contents);

            if (pluginType.Lifecycle != null)
            {
                _writer.AddContent("Scoped as:  " + pluginType.Lifecycle.GetType().Name);
            }
            else
            {
                _writer.AddContent("Scoped as:  PerRequest");
            }

            foreach (IInstance instance in pluginType.Instances)
            {
                writeInstance(instance);
            }
        }

        private void writeInstance(IInstance instance)
        {
            if (_instances.Contains(instance))
            {
                return;
            }

            var contents = new[] {string.Empty, string.Empty, string.Empty};
            setContents(contents, instance);

            _writer.AddText(contents);
        }


        private void setContents(string[] contents, IInstance instance)
        {
            contents[1] = instance.Name;
            contents[2] = instance.Description;

            _instances.Add(instance);
        }
    }
}