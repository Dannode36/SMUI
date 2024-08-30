using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class StaticContainer : Container
    {
        public Vector2 Size;

        public Color? OutlineColor = null;

        public override int Width => (int)Size.X;

        public override int Height => (int)Size.Y;

        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;
            if (OutlineColor.HasValue)
            {
                IClickableMenu.drawTextureBox(b, (int)Position.X - 12, (int)Position.Y - 12, Width + 24, Height + 24, OutlineColor.Value);
            }
            base.Draw(b);
        }
    }
}
