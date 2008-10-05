namespace StructureMap.Diagnostics
{
    internal class CharacterWidth
    {
        private int _width;

        internal int Width
        {
            get { return _width; }
        }

        internal static CharacterWidth[] For(int count)
        {
            var widths = new CharacterWidth[count];
            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = new CharacterWidth();
            }

            return widths;
        }

        internal void SetWidth(int width)
        {
            if (width > _width)
            {
                _width = width;
            }
        }

        internal void Add(int add)
        {
            _width += add;
        }
    }
}