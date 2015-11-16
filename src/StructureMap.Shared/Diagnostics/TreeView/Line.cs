namespace StructureMap.Diagnostics.TreeView
{
    public class Line
    {
        public Line(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        public void Prepend(string buffer)
        {
            Text = buffer + Text;
        }

        public int Length
        {
            get
            {
                return Text.Length;
            }
        }
    }
}