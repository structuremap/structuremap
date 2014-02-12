namespace StructureMap.Diagnostics
{
    public class ComplexPadding : ILeftPadding
    {
        private readonly ILeftPadding _inner;
        private readonly ILeftPadding _outer;

        public ComplexPadding(ILeftPadding inner, int spaces, string leftBorder = "")
        {
            _inner = inner;
            _outer = new LeftPadding(spaces, leftBorder);
        }

        public string Create()
        {
            return _inner.Create() + _outer.Create();
        }

        public ILeftPadding ToChild(int spaces, string leftBorder = "")
        {
            return new ComplexPadding(this, spaces, leftBorder);
        }
    }
}