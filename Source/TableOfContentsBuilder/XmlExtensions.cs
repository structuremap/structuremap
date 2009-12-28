using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace TableOfContentsBuilder
{
    public static class XmlExtensions
    {
        public static XmlElement With(this XmlElement node, Action<XmlElement> action)
        {
            action(node);
            return node;
        }

        public static XmlDocument FromFile(this XmlDocument document, string fileName)
        {
            document.Load(fileName);
            return document;
        }

        public static XmlElement WithRoot(this XmlDocument document, string elementName)
        {
            XmlElement element = document.CreateElement(elementName);
            document.AppendChild(element);

            return element;
        }


        public static XmlDocument WithXmlText(this XmlDocument document, string xml)
        {
            document.LoadXml(xml);

            return document;
        }

        public static XmlElement WithFormattedText(this XmlElement element, string text)
        {
            XmlCDataSection section = element.OwnerDocument.CreateCDataSection(text);
            element.AppendChild(section);

            return element;
        }

        public static XmlElement AddElement(this XmlNode element, string name)
        {
            XmlElement child = element.OwnerDocument.CreateElement(name);
            element.AppendChild(child);

            return child;
        }

        public static void AddComment(this XmlNode element, string text)
        {
            XmlComment comment = element.OwnerDocument.CreateComment(text);
            element.AppendChild(comment);
        }

        public static XmlElement AddElement(this XmlNode element, string name, Action<XmlElement> action)
        {
            XmlElement child = element.OwnerDocument.CreateElement(name);
            element.AppendChild(child);

            action(child);

            return child;
        }

        public static XmlElement WithInnerText(this XmlElement node, string text)
        {
            node.InnerText = text;
            return node;
        }

        public static XmlElement WithAtt(this XmlElement element, string key, string attValue)
        {
            element.SetAttribute(key, attValue);
            return element;
        }

        public static XmlElement WithAttributes(this XmlElement element, string text)
        {
            string[] atts = text.Split(',');
            foreach (string att in atts)
            {
                string[] parts = att.Split(':');

                element.WithAtt(parts[0].Trim(), parts[1].Trim());
            }

            return element;
        }

        public static void SetAttributeOnChild(this XmlElement element, string childName, string attName,
                                               string attValue)
        {
            XmlElement childElement = element[childName];
            if (childElement == null)
            {
                childElement = element.AddElement(childName);
            }

            childElement.SetAttribute(attName, attValue);
        }

        public static XmlElement WithProperties(this XmlElement element, Dictionary<string, string> properties)
        {
            foreach (var pair in properties)
            {
                element.SetAttribute(pair.Key, pair.Value);
            }

            return element;
        }

        public static XmlElement WithProperties(this XmlElement element, Cache<string, string> properties)
        {
            properties.Each((k, v) => element.SetAttribute(k, v));

            return element;
        }

        public static void ForEachElement(this XmlNode node, Action<XmlElement> action)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                var element = child as XmlElement;
                if (element != null)
                {
                    action(element);
                }
            }
        }
    }

    public class Cache<KEY, VALUE> : IEnumerable<VALUE> where VALUE : class
    {
        private readonly object _locker = new object();
        private readonly IDictionary<KEY, VALUE> _values;

        private Func<VALUE, KEY> _getKey = delegate { throw new NotImplementedException(); };

        private Func<KEY, VALUE> _onMissing = delegate(KEY key)
        {
            string message = string.Format("Key '{0}' could not be found", key);
            throw new KeyNotFoundException(message);
        };

        public Cache()
            : this(new Dictionary<KEY, VALUE>())
        {
        }

        public Cache(Func<KEY, VALUE> onMissing)
            : this(new Dictionary<KEY, VALUE>(), onMissing)
        {
        }

        public Cache(IDictionary<KEY, VALUE> dictionary, Func<KEY, VALUE> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public Cache(IDictionary<KEY, VALUE> dictionary)
        {
            _values = dictionary;
        }

        public Func<KEY, VALUE> OnMissing { set { _onMissing = value; } }

        public Func<VALUE, KEY> GetKey { get { return _getKey; } set { _getKey = value; } }

        public int Count { get { return _values.Count; } }

        public VALUE First
        {
            get
            {
                foreach (var pair in _values)
                {
                    return pair.Value;
                }

                return null;
            }
        }


        public VALUE this[KEY key]
        {
            get
            {
                if (!_values.ContainsKey(key))
                {
                    lock (_locker)
                    {
                        if (!_values.ContainsKey(key))
                        {
                            VALUE value = _onMissing(key);
                            _values.Add(key, value);
                        }
                    }
                }

                return _values[key];
            }
            set
            {
                if (_values.ContainsKey(key))
                {
                    _values[key] = value;
                }
                else
                {
                    _values.Add(key, value);
                }
            }
        }

        #region IEnumerable<VALUE> Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<VALUE>) this).GetEnumerator();
        }

        public IEnumerator<VALUE> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        #endregion

        public void Fill(KEY key, VALUE value)
        {
            if (_values.ContainsKey(key))
            {
                return;
            }

            _values.Add(key, value);
        }

        public bool TryRetrieve(KEY key, out VALUE value)
        {
            value = default(VALUE);

            if (_values.ContainsKey(key))
            {
                value = _values[key];
                return true;
            }

            return false;
        }

        public void Each(Action<VALUE> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Value);
            }
        }

        public void Each(Action<KEY, VALUE> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Key, pair.Value);
            }
        }

        public bool Has(KEY key)
        {
            return _values.ContainsKey(key);
        }

        public bool Exists(Predicate<VALUE> predicate)
        {
            bool returnValue = false;

            Each(delegate(VALUE value) { returnValue |= predicate(value); });

            return returnValue;
        }

        public VALUE Find(Predicate<VALUE> predicate)
        {
            foreach (var pair in _values)
            {
                if (predicate(pair.Value))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public VALUE[] GetAll()
        {
            var returnValue = new VALUE[Count];
            _values.Values.CopyTo(returnValue, 0);

            return returnValue;
        }

        public void Remove(KEY key)
        {
            if (_values.ContainsKey(key))
            {
                _values.Remove(key);
            }
        }

        public void ClearAll()
        {
            _values.Clear();
        }
    }
}