using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class WhatDoIHaveWriter : IPipelineGraphVisitor
    {
        private readonly PipelineGraph _graph;
        private TextReportWriter _writer;
        private List<IInstance> _instances;

        public WhatDoIHaveWriter(PipelineGraph graph)
        {
            _graph = graph;

        }

        public string GetText()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

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

            foreach (var pluginType in _graph.PluginTypes)
            {
                writePluginType(pluginType);
            }

            _writer.AddDivider('=');

            _writer.Write(stringWriter);
        }

        private void writeSources(StringWriter writer)
        {
            writer.WriteLine("===========================================================================================================");
            writer.WriteLine("Configuration Sources:");
            writer.WriteLine();

            for (int i = 0; i < _graph.Log.Sources.Length; i++)
            {
                var source = _graph.Log.Sources[i];
                string message = (i.ToString() + ")").PadRight(5) + source;
                writer.WriteLine(message);
            }

            writer.WriteLine();
        }

        private void writePluginType(PluginTypeConfiguration pluginType)
        {
            _writer.AddDivider('-');
            string[] contents = new string[] { pluginType.PluginType.AssemblyQualifiedName ?? pluginType.PluginType.Name, string.Empty, string.Empty };

            if (pluginType.Default != null)
            {
                setContents(contents, pluginType.Default);

            }

            _writer.AddText(contents);

            _writer.AddContent("Built by:  " + pluginType.Policy.ToString());

            foreach (var instance in pluginType.Instances)
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

            string[] contents = new[] { string.Empty, string.Empty, string.Empty };
            setContents(contents, instance);

            _writer.AddText(contents);
        }


        void IPipelineGraphVisitor.PluginType(Type pluginType, Instance defaultInstance, IBuildPolicy policy)
        {
            _writer.AddDivider('-');
            string[] contents = new string[]{TypePath.GetAssemblyQualifiedName(pluginType), string.Empty, string.Empty};

            if (defaultInstance != null)
            {
                setContents(contents, defaultInstance);
                
            }

            _writer.AddText(contents);

            _writer.AddContent("Built by:  " + policy.ToString());
        }

        private void setContents(string[] contents, IInstance instance)
        {
            contents[1] = instance.Name;
            contents[2] = instance.Description;

            _instances.Add(instance);
        }

        void IPipelineGraphVisitor.Instance(Type pluginType, Instance instance)
        {
            if (_instances.Contains(instance))
            {
                return;
            }

            string[] contents = new string[]{string.Empty, string.Empty, string.Empty};
            setContents(contents, instance);

            _writer.AddText(contents);
        }

    }
}
