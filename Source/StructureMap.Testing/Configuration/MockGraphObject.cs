using System.Collections;
using NUnit.Framework;
using StructureMap.Configuration;

namespace StructureMap.Testing.Configuration
{
    public class MockGraphObject : GraphObject
    {
        private bool _acceptWasCalled = false;
        private ArrayList _children = new ArrayList();

        public MockGraphObject()
        {
        }

        public override GraphObject[] Children
        {
            get { return (GraphObject[]) _children.ToArray(typeof (GraphObject)); }
        }

        protected override string key
        {
            get { return string.Empty; }
        }

        public void AddChild()
        {
            MockGraphObject child = new MockGraphObject();
            _children.Add(child);
        }

        public void AddDescendant(int[] indices)
        {
            MockGraphObject node = this;

            for (int i = 0; i < indices.Length; i++)
            {
                node = (MockGraphObject) node._children[indices[i]];
            }

            node.AddChild();
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            _acceptWasCalled = true;
        }

        public void VerifyAcceptVisitor()
        {
            Assert.IsTrue(_acceptWasCalled);

            foreach (MockGraphObject child in _children)
            {
                child.VerifyAcceptVisitor();
            }
        }
    }
}