using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                enumerable = enumerable.Where(x => x.PluginType.GetTypeInfo().Assembly == Assembly);
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
        private readonly StringWriter _stringWriter = new StringWriter();

        public WhatDoIHaveWriter(IPipelineGraph graph)
        {
            _graph = graph;
        }

        public string GetText(ModelQuery query, string title = null)
        {
            if (title.IsNotEmpty())
            {
                _stringWriter.WriteLine(title);
            }

            switch (_graph.Role)
            {
                case ContainerRole.Root:
                    break;
                case ContainerRole.ProfileOrChild:
                    _stringWriter.WriteLine("Profile is '{0}'", _graph.Profile);
                    break;

                case ContainerRole.Nested:
                    _stringWriter.WriteLine("Nested Container: " + _graph.Profile);
                    break;
            }

            _stringWriter.WriteLine("");

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
            _writer.AddText("PluginType", "Namespace", "Lifecycle", "Description", "Name");

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
                if (contents[4].IsEmpty())
                {
                    contents[4] = "(Default)";
                }
                else
                {
                    contents[4] += " (Default)";
                }
            }

            _writer.AddText(contents);

            foreach (var instance in pluginType.Instances)
            {
                writeInstance(instance);
            }

            if (pluginType.MissingNamedInstance != null)
            {
                writeInstance(pluginType.MissingNamedInstance, "*Missing Named Instance*");
            }

            if (pluginType.Fallback != null)
            {
                writeInstance(pluginType.Fallback, "*Fallback*");
            }
        }

        private void writeInstance(InstanceRef instance, string name = null)
        {
            if (_instances.Contains(instance.Instance) || instance.Instance == null)
            {
                return;
            }

            var contents = new[] {string.Empty, string.Empty, string.Empty, string.Empty, string.Empty};
            setContents(contents, instance, name);

            _writer.AddText(contents);
        }


        private void setContents(string[] contents, InstanceRef instance, string name = null)
        {
            contents[2] = instance.Lifecycle.ToName();

            contents[3] = instance.Description;

            if (name.IsNotEmpty())
            {
                contents[4] = name;
            }
            else
            {
                Guid assignedName;
                if (instance.Instance.HasExplicitName() && !Guid.TryParse(instance.Name, out assignedName))
                {
                    contents[4] = instance.Name;
                }
            }



            if (contents[4].Length > 30)
            {
                contents[4] = contents[4].Substring(0, 27) + "...";
            }


            _instances.Add(instance.Instance);
        }
    }
}