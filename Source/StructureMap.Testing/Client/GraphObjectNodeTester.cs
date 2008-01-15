using NUnit.Framework;
using StructureMap.Client.TreeNodes;
using StructureMap.Configuration;

namespace StructureMap.Testing.Client
{
    [TestFixture]
    public class GraphObjectNodeTester
    {
        [Test]
        public void FindById()
        {
            StubGraphObject object1 = new StubGraphObject(0);
            StubGraphObject object2 = new StubGraphObject(0);
            StubGraphObject object3 = new StubGraphObject(0);
            StubGraphObject object4 = new StubGraphObject(0);
            StubGraphObject object5 = new StubGraphObject(0);
            StubGraphObject object6 = new StubGraphObject(0);

            GraphObjectNode node1 = new GraphObjectNode("", object1, "");
            GraphObjectNode node2 = new GraphObjectNode("", object2, "");
            GraphObjectNode node3 = new GraphObjectNode("", object3, "");
            GraphObjectNode node4 = new GraphObjectNode("", object4, "");
            GraphObjectNode node5 = new GraphObjectNode("", object5, "");
            GraphObjectNode node6 = new GraphObjectNode("", object6, "");

            node1.Nodes.Add(node2);
            node1.Nodes.Add(node3);
            node3.Nodes.Add(node4);
            node3.Nodes.Add(node5);
            node5.Nodes.Add(node6);

            Assert.AreEqual(node1, node1.FindById(object1.Id));
            Assert.AreEqual(node2, node1.FindById(object2.Id));
            Assert.AreEqual(node3, node1.FindById(object3.Id));
            Assert.AreEqual(node4, node1.FindById(object4.Id));
            Assert.AreEqual(node5, node1.FindById(object5.Id));
            Assert.AreEqual(node6, node1.FindById(object6.Id));
        }

        [Test]
        public void HasProblemsFindOneLevelOfChildren()
        {
            StubGraphObject object1 = new StubGraphObject(0);
            GraphObjectNode node1 = new GraphObjectNode("text", object1, "view");

            StubGraphObject object2 = new StubGraphObject(0);
            GraphObjectNode node2 = new GraphObjectNode("text", object2, "view");

            node1.Nodes.Add(node2);

            Assert.IsFalse(node1.HasProblems);

            object2.LogProblem(new Problem("text", string.Empty));

            Assert.IsTrue(node1.HasProblems);
        }

        [Test]
        public void HasProblemsFindTwoLevelOfChildren()
        {
            StubGraphObject object1 = new StubGraphObject(0);
            GraphObjectNode node1 = new GraphObjectNode("text", object1, "view");

            StubGraphObject object2 = new StubGraphObject(0);
            GraphObjectNode node2 = new GraphObjectNode("text", object2, "view");

            StubGraphObject object3 = new StubGraphObject(0);
            GraphObjectNode node3 = new GraphObjectNode("text", object3, "view");

            node1.Nodes.Add(node2);
            node2.Nodes.Add(node3);


            Assert.IsFalse(node1.HasProblems);

            object3.LogProblem(new Problem("text", string.Empty));

            Assert.IsTrue(node1.HasProblems);
        }

        [Test]
        public void HasProblemsWithNoChildren()
        {
            StubGraphObject object1 = new StubGraphObject(0);
            GraphObjectNode node = new GraphObjectNode("text", object1, "view");
            Assert.IsFalse(node.HasProblems);

            StubGraphObject object2 = new StubGraphObject(1);
            GraphObjectNode node2 = new GraphObjectNode("text", object2, "view");
            Assert.IsTrue(node2.HasProblems);
        }
    }

    public class StubGraphObject : GraphObject
    {
        public StubGraphObject(int problemCount)
        {
            for (int i = 0; i < problemCount; i++)
            {
                Problem problem = new Problem(i.ToString(), string.Empty);
                LogProblem(problem);
            }
        }

        protected override string key
        {
            get { return "KEY"; }
        }
    }
}