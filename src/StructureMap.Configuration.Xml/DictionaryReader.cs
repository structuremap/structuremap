using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.Xml
{
    public class DictionaryReader : ITypeReader<XmlNode>
    {
        #region ITypeReader Members

        public bool CanProcess(Type pluginType)
        {
            if (pluginType.Equals(typeof (NameValueCollection))) return true;
            if (!pluginType.IsGenericType) return false;

            Type definition = pluginType.GetGenericTypeDefinition();
            if (definition == null) return false;

            return definition.Equals(typeof (IDictionary<,>)) || definition.Equals(typeof (Dictionary<,>));
        }

        public Instance Read(XmlNode node, Type pluginType)
        {
            IBuilder builder = findBuilder(pluginType);
            node.ForEachChild("Pair").Do(
                element => builder.Read(element.GetAttribute("Key"), element.GetAttribute("Value")));

            return new SerializedInstance(builder.Object);
        }

        #endregion

        private static IBuilder findBuilder(Type pluginType)
        {
            if (pluginType.Equals(typeof (NameValueCollection))) return new NameValueCollectionBuilder();

            Type definition = pluginType.GetGenericTypeDefinition();
            if (definition.Equals(typeof (IDictionary<,>)) || definition.Equals(typeof (Dictionary<,>)))
            {
                Type[] arguments = pluginType.GetGenericArguments();
                Type builderType = typeof (DictionaryBuilder<,>).MakeGenericType(arguments);
                return (IBuilder) Activator.CreateInstance(builderType);
            }

            return null;
        }

        #region Nested type: DictionaryBuilder

        private class DictionaryBuilder<KEY, VALUE> : IBuilder
        {
            private readonly Dictionary<KEY, VALUE> _dictionary = new Dictionary<KEY, VALUE>();

            #region IBuilder Members

            public void Read(string name, string value)
            {
                var key = (KEY) Convert.ChangeType(name, typeof (KEY));
                var theValue = (VALUE) Convert.ChangeType(value, typeof (VALUE));

                _dictionary.Add(key, theValue);
            }

            public object Object { get { return _dictionary; } }

            #endregion
        }

        #endregion

        #region Nested type: IBuilder

        private interface IBuilder
        {
            object Object { get; }
            void Read(string name, string value);
        }

        #endregion

        #region Nested type: NameValueCollectionBuilder

        private class NameValueCollectionBuilder : IBuilder
        {
            private readonly NameValueCollection _collection = new NameValueCollection();

            #region IBuilder Members

            public void Read(string name, string value)
            {
                _collection.Add(name, value);
            }

            public object Object { get { return _collection; } }

            #endregion
        }

        #endregion
    }
}