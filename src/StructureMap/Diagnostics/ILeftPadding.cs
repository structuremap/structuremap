namespace StructureMap.Diagnostics
{
    public interface ILeftPadding
    {
        string Create();
    }

    public static class LeftPaddingExtensions
    {
        public static ILeftPadding ToChild(this ILeftPadding padding, int spaces, string leftBorder = "")
        {
            return new ComplexPadding(padding, spaces, leftBorder);
        }

        public static ILeftPadding ToChild(this ILeftPadding padding, ILeftPadding child)
        {
            return new ComplexPadding(padding, child);
        }
    }
}