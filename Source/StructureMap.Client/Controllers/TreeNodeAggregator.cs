using System;
using System.Collections;
using StructureMap.Client.TreeNodes;

namespace StructureMap.Client.Controllers
{
	public class TreeNodeAggregator
	{
		private readonly Type _subjectType;
		private readonly string _text;
		private readonly string _viewName;

		public TreeNodeAggregator(Type subjectType, string text, string viewName)
		{
			_subjectType = subjectType;
			_text = text;
			_viewName = viewName;
		}

		public GraphObjectNode Aggregate(GraphObjectNode parentNode)
		{
			GraphObjectNode aggregateNode = new GraphObjectNode(_text, parentNode.Subject, _viewName);
			aggregateNode.IsAggregate = true;
			ArrayList list = new ArrayList();

			foreach (GraphObjectNode child in parentNode.Nodes)
			{
				if (_subjectType.Equals(child.Subject.GetType()))
				{
					list.Add(child);
				}
			}

			foreach (GraphObjectNode child in list)
			{
				parentNode.Nodes.Remove(child);
				aggregateNode.Nodes.Add(child);
			}

			parentNode.Nodes.Add(aggregateNode);

			return aggregateNode;
		}
	}
}
