namespace StructureMap.Diagnostics
{
    public class ComplexPadding : ILeftPadding
    {
        private readonly ILeftPadding _inner;
        private readonly ILeftPadding _outer;

        public ComplexPadding(ILeftPadding inner, int spaces, string leftBorder = "")
            : this(inner, new LeftPadding(spaces, leftBorder))
        {
        }

        public ComplexPadding(ILeftPadding inner, ILeftPadding outer)
        {
            _inner = inner;
            _outer = outer;
        }

        public string Create()
        {
            return _inner.Create() + _outer.Create();
        }
    }
}