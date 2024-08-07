using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class Scrollbar : Element
    {
        private bool DragScroll;

        public int UserHeight { get; set; }

        public int Rows { get; set; }
        public int FrameSize { get; set; }

        public int TopRow { get; private set; }
        public int MaxTopRow => Math.Max(0, Rows - FrameSize);

        public float ScrollPercent => (MaxTopRow > 0) ? TopRow / (float)MaxTopRow : 0f;

        public override int Width => 24;
        public override int Height => UserHeight;

        public void ScrollBy(int amount)
        {
            int row = Utilities.Clamp(0, TopRow + amount, MaxTopRow);
            if (row != TopRow)
            {
                Game1.playSound("shwip");
                TopRow = row;
            }
        }

        public void ScrollTo(int row)
        {
            if (TopRow != row)
            {
                Game1.playSound("shiny4");
                TopRow = Utilities.Clamp(0, row, MaxTopRow);
            }
        }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            if (Clicked)
            {
                DragScroll = true;
            }

            if (Constants.TargetPlatform != GamePlatform.Android)
            {
                if (DragScroll && Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    DragScroll = false;
                }
            }
            else
            {
                if (DragScroll && Game1.input.GetMouseState().LeftButton == ButtonState.Released)
                {
                    DragScroll = false;
                }
            }


            if (DragScroll)
            {
                int my = Game1.getMouseY();
                int relY = (int)(my - Position.Y - 40 / 2);
                ScrollTo((int)Math.Round(relY / (float)(Height - 40) * MaxTopRow));
            }
        }

        public override void Draw(SpriteBatch b)
        {
            if (IsHidden()) { return; }

            // Don't draw a scrollbar if scrolling is (currently) not possible.
            if (MaxTopRow == 0) { return; }

            Rectangle back = new((int)Position.X, (int)Position.Y, Width, Height);
            Vector2 front = new(back.X, back.Y + (Height - 40) * ScrollPercent);

            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), back.X, back.Y, back.Width, back.Height, Color.White, Game1.pixelZoom, false);
            b.Draw(Game1.mouseCursors, front, new Rectangle(435, 463, 6, 12), Color.White, 0f, new Vector2(), Game1.pixelZoom, SpriteEffects.None, 0.77f);
        }
    }
}
