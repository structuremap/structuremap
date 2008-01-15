using System;
using System.Windows.Forms;
using StructureMap.Configuration;

namespace StructureMap.Client.TreeNodes
{
    public class GraphObjectNode : TreeNode
    {
        private const int GREEN = 0;
        private const int RED = 1;

        private readonly GraphObject _subject;
        private readonly string _viewName;
        private bool _isAggregate = false;


        public GraphObjectNode(string text, GraphObject subject, string viewName)
        {
            Text = text;
            _subject = subject;
            _viewName = viewName;
        }

        public GraphObject Subject
        {
            get { return _subject; }
        }

        public string ViewName
        {
            get { return _viewName; }
        }

        public bool HasProblems
        {
            get
            {
                bool returnValue = (_subject.Problems.Length > 0);

                if (!returnValue)
                {
                    return HasChildrenProblems;
                }

                return returnValue;
            }
        }

        public bool HasChildrenProblems
        {
            get
            {
                foreach (GraphObjectNode child in Nodes)
                {
                    if (child.HasProblems)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsAggregate
        {
            get { return _isAggregate; }
            set { _isAggregate = value; }
        }

        public GraphObjectNode FindChild(string text)
        {
            foreach (GraphObjectNode child in Nodes)
            {
                if (child.Text.ToLower() == text.ToLower())
                {
                    return child;
                }
            }

            return null;
        }

        public GraphObjectNode FindById(Guid id)
        {
            if (_subject.Id == id)
            {
                return this;
            }

            foreach (GraphObjectNode child in Nodes)
            {
                GraphObjectNode node = child.FindById(id);
                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        public void RefreshStatus()
        {
            bool hasProblems = IsAggregate ? HasChildrenProblems : HasProblems;

            if (hasProblems)
            {
                ImageIndex = RED;
                SelectedImageIndex = RED;
            }
            else
            {
                ImageIndex = GREEN;
                SelectedImageIndex = GREEN;
            }

            foreach (GraphObjectNode child in Nodes)
            {
                child.RefreshStatus();
            }
        }
    }
}