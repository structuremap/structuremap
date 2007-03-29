using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Configuration
{
    [Serializable]
    public abstract class GraphObject : IComparable
    {
        private List<Problem> _problems = new List<Problem>();
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
            get { return _problems.ToArray(); }
            set { _problems = new List<Problem>(value); }
        }

        public void LogProblem(Problem problem)
        {
            _problems.Add(problem);
        }

        public virtual GraphObject[] Children
        {
            get { return new GraphObject[0]; }
        }

        public virtual void AcceptVisitor(IConfigurationVisitor visitor)
        {
            // no-op
        }

        public int CompareTo(object obj)
        {
            GraphObject peer = (GraphObject) obj;
            return key.CompareTo(peer.key);
        }

        protected abstract string key { get; }
    }
}