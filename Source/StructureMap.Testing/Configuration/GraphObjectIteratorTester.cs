using NMock;
using NMock.Constraints;
using NUnit.Framework;
using StructureMap.Configuration;

namespace StructureMap.Testing.Configuration
{
	[TestFixture]
	public class GraphObjectIteratorTester
	{
		[Test]
		public void IterateThroughChildren()
		{
			MockGraphObject node = new MockGraphObject();
			node.AddChild();
			node.AddChild();
			node.AddChild();
			node.AddDescendant(new int[]{0});
			node.AddDescendant(new int[]{0});
			node.AddDescendant(new int[]{0,0});
			node.AddDescendant(new int[]{0,1});
			node.AddDescendant(new int[]{0,1,0});
			node.AddDescendant(new int[]{0,1,0,0});

			DynamicMock visitorMock = new DynamicMock(typeof(IConfigurationVisitor));
			for (int i = 0; i < 10; i++)
			{
				visitorMock.Expect("StartObject", new IsTypeOf(typeof(GraphObject)));
				visitorMock.Expect("EndObject", new IsTypeOf(typeof(GraphObject	)));
			}

			GraphObjectIterator iterator = new GraphObjectIterator((IConfigurationVisitor) visitorMock.MockInstance);
			iterator.Visit(node);

			node.VerifyAcceptVisitor();
			visitorMock.Verify();
		}
	}
}
