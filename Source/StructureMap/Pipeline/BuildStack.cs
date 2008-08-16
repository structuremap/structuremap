using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class BuildStack
    {
        private readonly Stack<BuildFrame> _frameStack = new Stack<BuildFrame>();
        private BuildFrame _root;

        internal BuildStack()
        {

        }

        public BuildFrame Root
        {
            get { return _root; }
        }

        public BuildFrame Current
        {
            get { return _frameStack.Peek(); }
        }

        internal void Push(BuildFrame frame)
        {
            if (_root == null) _root = frame;
            _frameStack.Push(frame);
        }

        internal void Pop()
        {
            _frameStack.Pop();
        }
    }
}