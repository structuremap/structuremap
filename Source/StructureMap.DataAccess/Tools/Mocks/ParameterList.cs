using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class ParameterList : ICloneable
    {
        private Hashtable _values;

        public ParameterList()
        {
            _values = new Hashtable();
        }

        [IndexerName("ParameterValue")]
        public object this[string parameterName]
        {
            get { return _values[parameterName]; }
            set { _values[parameterName] = value; }
        }

        public string[] AllKeys
        {
            get
            {
                var returnValue = new string[_values.Count];
                _values.Keys.CopyTo(returnValue, 0);

                Array.Sort(returnValue);

                return returnValue;
            }
        }

        #region ICloneable Members

        public object Clone()
        {
            var clone = new ParameterList();
            clone._values = (Hashtable) _values.Clone();

            return clone;
        }

        #endregion

        public void Verify(ParameterList actualList)
        {
            var expectedKeys = new ArrayList(AllKeys);
            var actualKeys = new ArrayList(actualList.AllKeys);
            var unionKeys = new ArrayList();

            var keys = (string[]) actualKeys.ToArray(typeof (string));

            foreach (string key in keys)
            {
                if (expectedKeys.Contains(key))
                {
                    unionKeys.Add(key);
                    expectedKeys.Remove(key);
                    actualKeys.Remove(key);
                }
            }

            var failureCondition = new ParameterValidationFailureException();

            checkForWrongParameterValues(unionKeys, actualList, failureCondition);
            checkForMissingParameters(expectedKeys, failureCondition);
            checkForUnExpectedParameters(actualList, actualKeys, failureCondition);

            failureCondition.ThrowIfExceptions();
        }

        private void checkForWrongParameterValues(ArrayList unionKeys, ParameterList actualList,
                                                  ParameterValidationFailureException failureCondition)
        {
            foreach (string key in unionKeys)
            {
                object expected = this[key];
                object actual = actualList[key];

                if (!expected.Equals(actual))
                {
                    failureCondition.MarkWrongParameterValue(key, expected, actual);
                }
            }
        }

        private void checkForMissingParameters(ArrayList keys, ParameterValidationFailureException condition)
        {
            foreach (string key in keys)
            {
                condition.MarkMissingParameter(key, this[key]);
            }
        }

        private void checkForUnExpectedParameters(ParameterList list, ArrayList keys,
                                                  ParameterValidationFailureException condition)
        {
            foreach (string key in keys)
            {
                condition.MarkUnexpectedParameter(key, list[key]);
            }
        }

        public bool Contains(string parameterName)
        {
            return _values.ContainsKey(parameterName);
        }
    }
}