using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    public class WhatDoIHaveWriter
    {
        private readonly IPipelineGraph _graph;
        private List<Instance> _instances;
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
            _writer = new TextReportWriter(5);
            _instances = new List<Instance>();

            _writer.AddDivider('=');
            _writer.AddText("PluginType", "Namespace", "Lifecycle", "Name", "Description");

            var model = _graph.ToModel();

            model.PluginTypes.OrderBy(x => x.PluginType.Name).Each(writePluginType);

            _writer.AddDivider('=');

            _writer.Write(stringWriter);
        }

        private void writePluginType(IPluginTypeConfiguration pluginType)
        {
            _writer.AddDivider('-');
            var contents = new[]
            {
                "{0}".ToFormat(pluginType.PluginType.GetTypeName()),
                pluginType.PluginType.Namespace,
                string.Empty,
                string.Empty,
                string.Empty
            };

            if (pluginType.Default != null)
            {
                setContents(contents, pluginType.Default);
                if (contents[3].IsEmpty())
                {
                    contents[3] = "(Default)";
                }
                else
                {
                    contents[3] += " (Default)";
                }

            }

            _writer.AddText(contents);

            foreach (var instance in pluginType.Instances)
            {
                writeInstance(instance);
            }
        }

        private void writeInstance(InstanceRef instance)
        {
            if (_instances.Contains(instance.Instance))
            {
                return;
            }

            var contents = new[] {string.Empty, string.Empty, string.Empty, string.Empty, string.Empty};
            setContents(contents, instance);

            _writer.AddText(contents);
        }


        private void setContents(string[] contents, InstanceRef instance)
        {
            contents[2] = instance.Lifecycle.ToName();

            if (instance.Instance.HasExplicitName())
            {
                contents[3] = instance.Name;
            }

            contents[4] = instance.Description;

            _instances.Add(instance.Instance);
        }
    }
}