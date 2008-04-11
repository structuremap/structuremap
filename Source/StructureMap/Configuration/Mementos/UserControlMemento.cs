using System;
using System.Web.UI;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.Mementos
{
    [Obsolete("Eliminate")]
    public class UserControlMemento : InstanceMemento
    {
        private readonly string _instanceKey;
        private string _url;

        public UserControlMemento(string instanceKey, string url)
        {
            _instanceKey = instanceKey;
            _url = url;
        }


        public UserControlMemento()
        {
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        protected override string innerConcreteKey
        {
            get { return string.Empty; }
        }

        protected override string innerInstanceKey
        {
            get { return _instanceKey; }
        }

        public override bool IsReference
        {
            get { return false; }
        }

        public override string ReferenceKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string getPropertyValue(string Key)
        {
            throw new NotImplementedException();
        }

        protected override InstanceMemento getChild(string Key)
        {
            throw new NotImplementedException();
        }

        public override InstanceMemento[] GetChildrenArray(string Key)
        {
            throw new NotImplementedException();
        }
    }
}