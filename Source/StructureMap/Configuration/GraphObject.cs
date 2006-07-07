using System;
using System.Collections;

namespace StructureMap.Configuration
{
	[Serializable]
	public abstract class GraphObject : IComparable
	{
		private ArrayList _problems = new ArrayList();
		private Guid _id = Guid.NewGuid();

		public GraphObject()
		{

		}

		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public Problem[] Problems
		{
			get
			{
				return (Problem[]) _problems.ToArray(typeof (Problem));
			}
			set
			{
				_problems = new ArrayList(value);
			}
		}

		public void LogProblem(Problem problem)
		{
			_problems.Add(problem);
		}

		public virtual GraphObject[] Children
		{
			get
			{
				return new GraphObject[0];
			}
		}

		public virtual void AcceptVisitor(IConfigurationVisitor visitor)
		{
			// no-op
		}

		public int CompareTo(object obj)
		{
			GraphObject peer = (GraphObject) obj;
			return this.key.CompareTo(peer.key);
		}

		protected abstract string key {get;}
	}
}
