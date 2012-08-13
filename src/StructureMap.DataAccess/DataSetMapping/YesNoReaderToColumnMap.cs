using System;
using System.Data;

namespace StructureMap.DataAccess.DataSetMapping
{
    public class YesNoReaderToColumnMap : ReaderToColumnMap
    {
        public const string NO = "N";
        public const string YES = "Y";

        public YesNoReaderToColumnMap(string readerName, string columnName) : base(readerName, columnName)
        {
        }

        protected override object getRawValue(IDataReader reader)
        {
            object rawValue = base.getRawValue(reader);
            if (DBNull.Value == rawValue)
            {
                return DBNull.Value;
            }

            string stringValue = rawValue.ToString();
            switch (stringValue)
            {
                case YES:
                    return true;

                case NO:
                    return false;

                default:
                    throw new ApplicationException("Invalid Y/N flag in IDataReader:  " + stringValue);
            }
        }
    }
}