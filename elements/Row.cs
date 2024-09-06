namespace SMUI.Elements
{
    public class Row : Container
    {
        public Row() { }
        public Row(Element[] elements)
        {
            foreach (Element e in elements)
            {
                AddChild(e);
            }
        }

        public override int Width => Parent == null ? 0 : Parent.Width;

        public override int Height => Parent == null ? 0 : (Parent as Table)?.RowHeight ?? 0; //Clear as mud?

        public int MaxChildHeight
        {
            get
            {
                int maxHeight = 0;
                foreach (Element e in Children)
                {
                    maxHeight = Math.Max(maxHeight, e.Height);
                }
                return maxHeight;
            }
        }
    }
}
