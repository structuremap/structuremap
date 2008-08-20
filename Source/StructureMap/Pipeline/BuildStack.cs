using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class BuildStack
    {
        private BuildFrame _root;
        private BuildFrame _current;

        internal BuildStack()
        {

        }

        public BuildFrame Root
        {
            get { return _root; }
        }

        public BuildFrame Current
        {
            get { return _current; }
        }

        public BuildFrame Parent
        {
            get
            {
                return _current.Parent;
            }
        }

        internal void Push(BuildFrame frame)
        {
            if (_root == null)
            {
                _root = _current = frame;
            }
            else
            {
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