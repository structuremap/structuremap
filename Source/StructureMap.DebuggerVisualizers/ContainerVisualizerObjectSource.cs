using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.DebuggerVisualizers;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap.DebuggerVisualizers
{
    public class ContainerVisualizerObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var container = target as Container;
            if (container == null)
                throw new InvalidOperationException("This visualizer does not support Type: " + target.GetType().Name);

            ContainerDetail details = BuildContainerDetails(container);
            Serialize(outgoingData, details);
        }

        public static ContainerDetail BuildContainerDetails(Container container)
        {
            IList<PluginTypeDetail> pluginTypeDetails = new List<PluginTypeDetail>();
            foreach (IPluginTypeConfiguration pluginType in container.Model.PluginTypes)
            {
                IList<InstanceDetail> instances = new List<InstanceDetail>();
                IList<InstanceRef> usedInstances = new List<InstanceRef>();

                if (pluginType.Default != null)
                {
                    instances.Add(buildInstanceDetail(pluginType.Default));
                    usedInstances.Add(pluginType.Default);
                }
                foreach (InstanceRef instance in pluginType.Instances)
                {
                    if (usedInstances.Contains(instance)) continue;
                    instances.Add(buildInstanceDetail(instance));
                }

                var pluginTypeDetail = new PluginTypeDetail(pluginType.PluginType, pluginType.Lifecycle.GetType(),
                                                            instances.ToArray());
                pluginTypeDetails.Add(pluginTypeDetail);
            }

            return new ContainerDetail(container.PluginGraph.Log.Sources, pluginTypeDetails.ToArray());
        }

        private static InstanceDetail buildInstanceDetail(InstanceRef instance)
        {
            return new InstanceDetail(instance.Name, instance.Description, instance.ConcreteType);
        }
    }
}