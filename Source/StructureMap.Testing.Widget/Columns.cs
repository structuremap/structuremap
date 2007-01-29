namespace StructureMap.Testing.Widget
{
    public interface Column
    {
        void Display();
    }

    public class BasicColumn : Column
    {
        #region Column Members

        public void Display()
        {
        }

        #endregion
    }


    public class DateColumn : Column
    {
        private string _HeaderName;
        private int _Width;
        private string _FieldName;

        public DateColumn(string HeaderName, int Width, string FieldName)
        {
            _HeaderName = HeaderName;
            _Width = Width;
            _FieldName = FieldName;
        }

        /// <summary>
        /// Just a shell to test whether the correct constructor is being called
        /// </summary>
        /// <param name="HeaderName"></param>
        public DateColumn(string HeaderName)
        {
        }

        public string HeaderName
        {
            get { return _HeaderName; }
        }

        public int Width
        {
            get { return _Width; }
        }

        public string FieldName
        {
            get { return _FieldName; }
        }

        #region Column Members

        public void Display()
        {
            // TODO:  Add DateColumn.Display implementation
        }

        #endregion
    }


    public class NumberColumn : Column
    {
        #region Column Members

        public void Display()
        {
            // TODO:  Add NumberColumn.Display implementation
        }

        #endregion
    }
}