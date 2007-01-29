using System;
using System.Collections;

namespace StructureMap.DataAccess
{
    public class TemplateParser
    {
        private readonly string _template;
        private int[] _leftCurlyIndexes;
        private int[] _rightCurlyIndexes;

        internal TemplateParser(string Template)
        {
            _template = Template;
        }

        internal string[] Parse()
        {
            _leftCurlyIndexes = findAllOccurrences("{");
            _rightCurlyIndexes = findAllOccurrences("}");

            if (_leftCurlyIndexes.Length != _rightCurlyIndexes.Length)
            {
                throw new ApplicationException(_template);
            }

            string[] tempValues = new string[_leftCurlyIndexes.Length];
            for (int i = 0; i < _leftCurlyIndexes.Length; i++)
            {
                tempValues[i] = getSubstitution(i);
            }

            ArrayList list = new ArrayList();
            foreach (string tempValue in tempValues)
            {
                if (!list.Contains(tempValue))
                {
                    list.Add(tempValue);
                }
            }

            return (string[]) list.ToArray(typeof (string));
        }

        private string getSubstitution(int index)
        {
            int leftCurlyIndex = _leftCurlyIndexes[index];
            int rightCurlyIndex = _rightCurlyIndexes[index];

            if (leftCurlyIndex > rightCurlyIndex)
            {
                throw new ApplicationException(_template);
            }

            return _template.Substring(leftCurlyIndex + 1, rightCurlyIndex - leftCurlyIndex - 1);
        }


        private int[] findAllOccurrences(string bracket)
        {
            ArrayList list = new ArrayList();

            int curlyIndex = _template.IndexOf(bracket, 0);
            while (curlyIndex > -1)
            {
                list.Add(curlyIndex);
                curlyIndex = _template.IndexOf(bracket, curlyIndex + 1);
            }

            return (int[]) list.ToArray(typeof (int));
        }
    }
}