using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class DictionaryReader : ITypeReader
    {
        public bool CanProcess(Type pluginType)
        {
            if (pluginType.Equals(typeof(NameValueCollection))) return true;
            if (!pluginType.IsGenericType) return false;

            var definition = pluginType.GetGenericTypeDefinition();
            if (definition == null) return false;

            return definition.Equals(typeof (IDictionary<,>)) || definition.Equals(typeof (Dictionary<,>));
        }

        private static IBuilder findBuilder(Type pluginType)
        {
            if (pluginType.Equals(typeof(NameValueCollection))) return new NameValueCollectionBuilder();

            var definition = pluginType.GetGenericTypeDefinition();
            if (definition.Equals(typeof(IDictionary<,>)) || definition.Equals(typeof(Dictionary<,>)))
            {
                var arguments = pluginType.GetGenericArguments();
                var builderType = typeof (DictionaryBuilder<,>).MakeGenericType(arguments);
                return (IBuilder) Activator.CreateInstance(builderType);
            }

            return null;
        }

        public Instance Read(XmlNode node, Type pluginType)
        {
            var builder = findBuilder(pluginType);
            node.ForEachChild("Pair").Do(element => builder.Read(element.GetAttribute("Key"), element.GetAttribute("Value")));

            return new SerializedInstance(builder.Object);
        }

       






        internal interface IBuilder
        {
            void Read(string name, string value);
            object Object { get; }
        }

        internal class NameValueCollectionBuilder : IBuilder
        {
            private readonly NameValueCollection _collection = new NameValueCollection();

            public void Read(string name, string value)
            {
                _collection.Add(name, value);
            }

            public object Object
            {
                get { return _collection; }
            }
        }

        internal class DictionaryBuilder<KEY, VALUE> : IBuilder
        {
            private Dictionary<KEY, VALUE> _dictionary = new Dictionary<KEY, VALUE>();

            public void Read(string name, string value)
            {
                KEY key = (KEY) Convert.ChangeType(name, typeof (KEY));
                VALUE theValue = (VALUE) Convert.ChangeType(value, typeof (VALUE));

                _dictionary.Add(key, theValue);
            }

            public object Object
            {
                get { return _dictionary; } 
            }
        }
    }
}