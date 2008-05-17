namespace StructureMap.Diagnostics
{
    internal class CharacterWidth
    {
        internal static CharacterWidth[] For(int count)
        {
            CharacterWidth[] widths = new CharacterWidth[count];
            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = new CharacterWidth();
            }

            return widths;
        }

        private int _width = 0;
    
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

        internal int Width
        {
            get
            {
                return _width;
            }
        }
    }
}