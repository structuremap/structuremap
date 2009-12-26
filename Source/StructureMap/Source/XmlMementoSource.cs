using System.Collections;
using System.Xml;

namespace StructureMap.Source
{
    /// <summary>
    /// Base class for all MementoSource classes that store InstanceMemento's as 
    /// node-normalized Xml
    /// </summary>
    public abstract class XmlMementoSource : MementoSource
    {
        private readonly string _keyAttribute;
        private readonly XmlMementoCreator _mementoCreator;
        private readonly string _nodeName;
        private readonly string _typeAttribute;
        private Hashtable _mementos;


        public XmlMementoSource(string NodeName, string TypeAttribute, string KeyAttribute, XmlMementoStyle style)
        {
            _nodeName = NodeName;
            _typeAttribute = TypeAttribute;
            _keyAttribute = KeyAttribute;

            _mementoCreator = new XmlMementoCreator(style, TypeAttribute, KeyAttribute);
        }

        public string NodeName { get { return _nodeName; } }

        public string TypeAttribute { get { return _typeAttribute; } }

        public string KeyAttribute { get { return _keyAttribute; } }

        private Hashtable mementoHashtable
        {
            get
            {
                if (_mementos == null)
                {
                    lock (this)
                    {
                        if (_mementos == null)
                        {
                            loadMementos();
                        }
                    }
                }

                return _mementos;
            }
        }

        private void loadMementos()
        {
            _mementos = new Hashtable();
            XmlNode root = getRootNode();

            if (root == null)
            {
                return;
            }

            XmlNode nodeChild = root.FirstChild;
            while (nodeChild != null)
            {
                if (nodeChild.Name == _nodeName)
                {
                    InstanceMemento memento = createMemento(nodeChild);
                    _mementos.Add(memento.InstanceKey, memento);
                }

                nodeChild = nodeChild.NextSibling;
            }
        }

        protected InstanceMemento createMemento(XmlNode nodeChild)
        {
            return _mementoCreator.CreateMemento(nodeChild);
        }


        protected abstract XmlNode getRootNode();

        protected override sealed InstanceMemento[] fetchInternalMementos()
        {
            Hashtable mementos = mementoHashtable;
            var returnValue = new InstanceMemento[mementos.Count];
            mementos.Values.CopyTo(returnValue, 0);

            return returnValue;
        }

        protected internal override bool containsKey(string Key)
        {
            return mementoHashtable.ContainsKey(Key);
        }

        protected override InstanceMemento retrieveMemento(string Key)
        {
            return mementoHashtable[Key] as InstanceMemento;
        }
    }
}