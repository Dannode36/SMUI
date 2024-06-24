using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SMUI.Elements
{
    public class Row : Container
    {
        public Row(Element[] elements)
        {
            Elements = elements;

            foreach (Element e in Elements)
            {
                AddChild(e);
            }
        }

        public Element[] Elements;
        public Vector2 ScrollOffset;

        public override int Width => Parent == null ? 0 : Parent.Width;

        public override int Height => Parent == null ? 0 : (Parent as Table)?.RowHeight ?? 0; //Clear as mud?

        public int MaxChildHeight
        {
            get
            {
                int maxHeight = 0;
                foreach (Element e in Elements)
                {
                    maxHeight = Math.Max(maxHeight, e.Height);
                }
                return maxHeight;
            }
        }
    }
}
