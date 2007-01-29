using System.Collections;
using NUnit.Framework;
using StructureMap.Client.TreeNodes;
using StructureMap.Configuration;

namespace StructureMap.Testing.Client.Controllers
{
    public class TreeNodeExpectation
    {
        private readonly string _text;
        private readonly string _view;
        private readonly GraphObject _subject;
        private ArrayList _children;

        public TreeNodeExpectation(string text, string view, GraphObject subject)
        {
            _text = text;
            _view = view;
            _subject = subject;

            _children = new ArrayList();
        }

        public void Verify(GraphObjectNode node)
        {
            Assert.AreEqual(_text, node.Text, "Text");
            Assert.AreEqual(_view, node.ViewName, "View");
            Assert.AreEqual(_subject, node.Subject, "Subject");

            Assert.AreEqual(_children.Count, node.Nodes.Count, "Number of children");

            for (int i = 0; i < _children.Count; i++)
            {
                TreeNodeExpectation childExpectation = this[i];
                GraphObjectNode childNode = (GraphObjectNode) node.Nodes[i];

                childExpectation.Verify(childNode);
            }
        }

        public TreeNodeExpectation AddChild(TreeNodeExpectation expectation)
        {
            _children.Add(expectation);
            return expectation;
        }

        public TreeNodeExpectation AddChild(string text, string view, GraphObject subject)
        {
            TreeNodeExpectation child = new TreeNodeExpectation(text, view, subject);
            AddChild(child);

            return child;
        }

        public TreeNodeExpectation this[int index]
        {
            get { return (TreeNodeExpectation) _children[index]; }
        }
    }
}