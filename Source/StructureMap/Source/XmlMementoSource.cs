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
		private string _nodeName;
		private string _typeAttribute;
		private string _keyAttribute;
		private Hashtable _mementos;
		private XmlMementoCreator _mementoCreator;
		

		public XmlMementoSource(string NodeName, string TypeAttribute, string KeyAttribute, XmlMementoStyle style) : base()
		{
			_nodeName = NodeName;
			_typeAttribute = TypeAttribute;
			_keyAttribute = KeyAttribute;

			_mementoCreator = new XmlMementoCreator(style, TypeAttribute, KeyAttribute);
		}

		public string NodeName
		{
			get { return _nodeName; }
		}

		public string TypeAttribute
		{
			get { return _typeAttribute; }
		}

		public string KeyAttribute
		{
			get { return _keyAttribute; }
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
				if (nodeChild.Name == this._nodeName)
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


		protected abstract XmlNode getRootNode();

		protected override sealed InstanceMemento[] fetchInternalMementos()
		{
			Hashtable mementos = this.mementoHashtable;
			InstanceMemento[] returnValue = new InstanceMemento[mementos.Count];
			mementos.Values.CopyTo(returnValue, 0);

			return returnValue;
		}

		protected internal override bool containsKey(string Key)
		{
			return this.mementoHashtable.ContainsKey(Key);
		}

		protected override InstanceMemento retrieveMemento(string Key)
		{
			return this.mementoHashtable[Key] as InstanceMemento;
		}
	}
}