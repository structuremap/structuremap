using System;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Provides metadata about the object graph being constructed.  More or less a stack trace of the GetInstance() pipeline
    /// that can be used for "contextual" object construction
    /// </summary>
    public class BuildStack
    {
        private BuildFrame _current;
        private BuildFrame _root;

        internal BuildStack()
        {
        }

        /// <summary>
        /// The top level of the object graph.  Describes the original requested instance
        /// </summary>
        public BuildFrame Root
        {
            get { return _root; }
        }

        /// <summary>
        /// The current BuildFrame
        /// </summary>
        public BuildFrame Current
        {
            get { return _current; }
        }

        /// <summary>
        /// The immediate parent BuildFrame
        /// </summary>
        public BuildFrame Parent
        {
            get { return _current.Parent; }
        }

        internal void Push(BuildFrame frame)
        {
            if (_root == null)
            {
                _root = _current = frame;
            }
            else
            {
                if (_root.Contains(frame))
                {


                    throw new StructureMapException(295, frame.ToString(), _root.ToStackString());
                }

                _current.Attach(frame);
                _current = frame;
            }
        }

        internal void Pop()
        {
            _current = _current.Detach();
            if (_current == null) _root = null;
        }
    }
}