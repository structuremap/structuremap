using System;
using System.Collections;
using System.Diagnostics;
using StructureMap.Client.TreeNodes;
using StructureMap.Client.Views;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Client.Controllers
{
	public class TreeBuilder : IConfigurationVisitor
	{
		private readonly PluginGraphReport _report;
		private GraphObjectNode _topNode;
		private Stack _nodes;

		public TreeBuilder(PluginGraphReport report)
		{
			_nodes = new Stack();

			_report = report;

			_topNode = new GraphObjectNode("PluginGraph", _report, ViewConstants.SUMMARY);
			_nodes.Push(_topNode);
		}

		public GraphObjectNode TopNode
		{
			get { return _topNode; }
		}

		public GraphObjectNode PushNodeOntoStack(string text, string view, GraphObject subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			GraphObjectNode node = new GraphObjectNode(text, subject, view);
			LastNode.Nodes.Add(node);
			_nodes.Push(node);

			Debug.WriteLine("Push:  " + subject.ToString() + ", count = " + _nodes.Count);

			return node;
		}

		public GraphObjectNode LastNode
		{
			get
			{
				if (_nodes.Count == 0)
				{
					return _topNode;
				}

				return (GraphObjectNode) _nodes.Peek();
			}
		}

		public void StartObject(GraphObject node)
		{
			// no-op;
		}

		public void EndObject(GraphObject node)
		{
			if (_nodes.Count == 0)
			{
				throw new ApplicationException("The stack is empty");
			}

			_nodes.Pop();

			Debug.WriteLine("Pop:  " + node.ToString() + ", count = " + _nodes.Count);

		}

		public void HandleAssembly(AssemblyToken assembly)
		{
			PushNodeOntoStack(assembly.AssemblyName, ViewConstants.ASSEMBLY, assembly);
		}

		public void HandleFamily(FamilyToken family)
		{
			PushNodeOntoStack(family.PluginType, ViewConstants.PLUGINFAMILY, family);
		}

		public void HandleMementoSource(MementoSourceInstanceToken source)
		{
			PushNodeOntoStack(ViewConstants.MEMENTO_SOURCE, ViewConstants.INSTANCE, source);
		}

		public void HandlePlugin(PluginToken plugin)
		{
			PushNodeOntoStack(plugin.ConcreteKey, ViewConstants.PLUGIN, plugin);
		}

		public void HandleInterceptor(InterceptorInstanceToken interceptor)
		{
			PushNodeOntoStack(interceptor.ConcreteKey, ViewConstants.INSTANCE, interceptor);
		}

		public void HandleInstance(InstanceToken instance)
		{
			PushNodeOntoStack(instance.InstanceKey, ViewConstants.INSTANCE, instance);
		}

		public void HandlePrimitiveProperty(PrimitiveProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.PRIMITIVE_PROPERTY, property);
		}

		public void HandleEnumerationProperty(EnumerationProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.ENUMERATION_PROPERTY, property);
		}

		public void HandleInlineChildProperty(ChildProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.INLINE_CHILD_PROPERTY, property);
		}

		public void HandleDefaultChildProperty(ChildProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.DEFAULT_CHILD_PROPERTY, property);
		}

		public void HandleReferenceChildProperty(ChildProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.REFERENCE_CHILD_PROPERTY, property);
		}

		public void HandlePropertyDefinition(PropertyDefinition propertyDefinition)
		{
			PushNodeOntoStack(propertyDefinition.PropertyName, ViewConstants.PROPERTY_DEFINITION, propertyDefinition);
		}

		public void HandleChildArrayProperty(ChildArrayProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.CHILD_ARRAY_PROPERTY, property);
		}

		public void HandleNotDefinedChildProperty(ChildProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.NOT_DEFINED_CHILD_PROPERTY, property);
		}

		public void HandleTemplate(TemplateToken template)
		{
			PushNodeOntoStack(template.TemplateKey, ViewConstants.TEMPLATE, template);
		}

		public void HandleTemplateProperty(TemplateProperty property)
		{
			PushNodeOntoStack(property.PropertyName, ViewConstants.TEMPLATE_PROPERTY, property);
		}


		public GraphObjectNode BuildTree()
		{
			GraphObjectIterator iterator = new GraphObjectIterator(this);
			iterator.Visit(_report);

			TreeNodeAggregator assemblyAggregator = new TreeNodeAggregator(typeof (AssemblyToken), ViewConstants.ASSEMBLIES, ViewConstants.ASSEMBLIES);
			assemblyAggregator.Aggregate(_topNode);

			TreeNodeAggregator familyAggregator = new TreeNodeAggregator(typeof (FamilyToken), ViewConstants.PLUGINFAMILIES, ViewConstants.PLUGINFAMILIES);
			GraphObjectNode familiesNode = familyAggregator.Aggregate(_topNode);

			TreeNodeAggregator[] aggregators = new TreeNodeAggregator[]
				{
					new TreeNodeAggregator(typeof (InterceptorInstanceToken), ViewConstants.INTERCEPTORS, ViewConstants.INTERCEPTORS),
					new TreeNodeAggregator(typeof (PluginToken), ViewConstants.PLUGINS, ViewConstants.PLUGINS),
					new TreeNodeAggregator(typeof (TemplateToken), ViewConstants.TEMPLATES, ViewConstants.TEMPLATES),
					new TreeNodeAggregator(typeof (InstanceToken), ViewConstants.INSTANCES, ViewConstants.INSTANCES)
				};

			foreach (GraphObjectNode familyNode in familiesNode.Nodes)
			{
				foreach (TreeNodeAggregator aggregator in aggregators)
				{
					aggregator.Aggregate(familyNode);
				}
			}

			_topNode.RefreshStatus();

			return _topNode;
		}
	}
}