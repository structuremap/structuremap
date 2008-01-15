using System;

namespace StructureMap.Configuration.Tokens
{
    [Serializable]
    public class TemplateToken : GraphObject
    {
        private string _concreteKey;
        private string _pluginType;
        private string[] _properties;
        private string _templateKey;

        public TemplateToken()
        {
        }

        public TemplateToken(string templateKey, string concreteKey, string[] properties)
        {
            _templateKey = templateKey;
            _concreteKey = concreteKey;
            _properties = properties;
        }

        public string TemplateKey
        {
            get { return _templateKey; }
            set { _templateKey = value; }
        }

        public string PluginType
        {
            get { return _pluginType; }
            set { _pluginType = value; }
        }

        public string ConcreteKey
        {
            get { return _concreteKey; }
            set { _concreteKey = value; }
        }

        public string[] Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }


        protected override string key
        {
            get { return _templateKey; }
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleTemplate(this);
        }
    }
}