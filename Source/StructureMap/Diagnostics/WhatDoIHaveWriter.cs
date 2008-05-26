using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class WhatDoIHaveWriter : IPipelineGraphVisitor
    {
        private readonly PipelineGraph _graph;
        private TextReportWriter _writer;
        private List<Instance> _instances;

        public WhatDoIHaveWriter(PipelineGraph graph)
        {
            _graph = graph;

        }

        public string GetText()
        {
            _writer = new TextReportWriter(3);
            _instances = new List<Instance>();

            _writer.AddDivider('=');
            _writer.AddText("PluginType", "Name", "Description"); 

            _graph.Visit(this);

            _writer.AddDivider('=');

            return _writer.Write();
        }

        void IPipelineGraphVisitor.PluginType(Type pluginType, Instance defaultInstance)
        {
            _writer.AddDivider('-');
            string[] contents = new string[]{TypePath.GetAssemblyQualifiedName(pluginType), string.Empty, string.Empty};

            if (defaultInstance != null)
            {
                setContents(contents, defaultInstance);
                
            }

            _writer.AddText(contents);
        }

        private void setContents(string[] contents, Instance instance)
        {
            InstanceToken token = ((IDiagnosticInstance)instance).CreateToken();
            contents[1] = token.Name;
            contents[2] = token.Description;

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
