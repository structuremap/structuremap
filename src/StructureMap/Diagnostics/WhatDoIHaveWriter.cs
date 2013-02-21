using System.Collections.Generic;
using System.IO;
using System.Text;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    public class WhatDoIHaveWriter
    {
        private readonly IPipelineGraph _graph;
        private List<InstanceRef> _instances;
        private TextReportWriter _writer;

        public WhatDoIHaveWriter(IPipelineGraph graph)
        {
            _graph = graph;
        }

        public string GetText()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            writeContentsOfPluginTypes(writer);

            return sb.ToString();
        }

        private void writeContentsOfPluginTypes(StringWriter stringWriter)
        {
            _writer = new TextReportWriter(3);
            _instances = new List<InstanceRef>();

            _writer.AddDivider('=');
            _writer.AddText("PluginType", "Name", "Description");

            foreach (IPluginTypeConfiguration pluginType in _graph.GetPluginTypes())
            {
                writePluginType(pluginType);
            }

            _writer.AddDivider('=');

            _writer.Write(stringWriter);
        }

        private void writePluginType(IPluginTypeConfiguration pluginType)
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
                _writer.AddContent("Scoped as:  " + pluginType.Lifecycle);
            }
            else
            {
                _writer.AddContent("Scoped as:  PerRequest/Transient");
            }

            foreach (InstanceRef instance in pluginType.Instances)
            {
                writeInstance(instance);
            }
        }

        private void writeInstance(InstanceRef instance)
        {
            if (_instances.Contains(instance))
            {
                return;
            }

            var contents = new[] {string.Empty, string.Empty, string.Empty};
            setContents(contents, instance);

            _writer.AddText(contents);
        }


        private void setContents(string[] contents, InstanceRef instance)
        {
            contents[1] = instance.Name;
            contents[2] = instance.Description;

            _instances.Add(instance);
        }
    }
}