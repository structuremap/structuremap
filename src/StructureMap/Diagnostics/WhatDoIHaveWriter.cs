using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    public class ModelQuery
    {
        public string Namespace;
        public Type PluginType;
        public Assembly Assembly;
        public string TypeName;

        public IEnumerable<IPluginTypeConfiguration> Query(IModel model)
        {
            var enumerable = model.PluginTypes;

            if (Namespace.IsNotEmpty())
            {
                enumerable = enumerable.Where(x => x.PluginType.IsInNamespace(Namespace));
            }

            if (PluginType != null)
            {
                enumerable = enumerable.Where(x => x.PluginType == PluginType);
            }

            if (Assembly != null)
            {
                enumerable = enumerable.Where(x => x.PluginType.Assembly == Assembly);
            }

            if (TypeName.IsNotEmpty())
            {
                enumerable = enumerable.Where(x => x.PluginType.Name.ToLower().Contains(TypeName.ToLower()));
            }

            return enumerable;
        } 
    }

    public class WhatDoIHaveWriter
    {
        private readonly IPipelineGraph _graph;
        private List<Instance> _instances;
        private TextReportWriter _writer;
        private StringWriter _stringWriter = new StringWriter();

        public WhatDoIHaveWriter(IPipelineGraph graph)
        {
            _graph = graph;
        }

        public string GetText(ModelQuery query)
        {
            var model = _graph.ToModel();

            var pluginTypes = query.Query(model);

            writeContentsOfPluginTypes(pluginTypes);

            return _stringWriter.ToString();
        }

        private void writeContentsOfPluginTypes(IEnumerable<IPluginTypeConfiguration> pluginTypes)
        {
            _writer = new TextReportWriter(5);
            _instances = new List<Instance>();

            _writer.AddDivider('=');
            _writer.AddText("PluginType", "Namespace", "Lifecycle", "Name", "Description");

            pluginTypes.OrderBy(x => x.PluginType.Name).Each(writePluginType);

            _writer.AddDivider('=');

            _writer.Write(_stringWriter);
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