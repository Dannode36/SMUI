namespace SMUI.Types
{
    public struct Margin
    {
        public int Left = 0;
        public int Right = 0;
        public int Top = 0;
        public int Bottom = 0;

        public Margin() { }
        public Margin(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }
}
