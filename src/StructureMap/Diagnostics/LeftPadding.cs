namespace StructureMap.Diagnostics
{
    public class LeftPadding : ILeftPadding
    {
        private readonly int _spaces;
        private readonly string _leftBorder;

        public LeftPadding(int spaces, string leftBorder = "")
        {
            _spaces = spaces;
            _leftBorder = leftBorder;
        }


        public string Create()
        {
            return _leftBorder.PadRight(_spaces);
        }
    }
}