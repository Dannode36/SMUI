using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class Slider : Element
    {
        protected bool Dragging;
        public int UserWidth { get; set; }
        public override int Width => UserWidth;
        public override int Height => 24;

        public Action<Element>? Callback { get; set; }

        public override void Draw(SpriteBatch b) { }
    }

    public class Slider<T> : Slider where T : struct
    {
        public T Minimum { get; set; }
        public T Maximum { get; set; }
        public T Value { get; set; }

        public T Interval { get; set; }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            if (Clicked)
            {
                Dragging = true;
            }

            if (Constants.TargetPlatform != GamePlatform.Android)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Released
                    && Game1.input.GetGamePadState().Buttons.A == ButtonState.Released)
                {
                    Dragging = false;
                }
            }
            else
            {
                if (Game1.input.GetMouseState().LeftButton == ButtonState.Released
                    && Game1.input.GetGamePadState().Buttons.A == ButtonState.Released)
                {
                    Dragging = false;
                }
            }

            if (Dragging)
            {
                float percent = (Game1.getOldMouseX() - Position.X) / Width;
                Value = Utilities.Adjust(Value, Interval);
                Value = Value switch
                {
                    int => Utilities.Clamp(Minimum, (T)(object)(int)(percent * ((int)(object)Maximum - (int)(object)Minimum) + (int)(object)Minimum), Maximum),
                    float => Utilities.Clamp(Minimum, (T)(object)(percent * ((float)(object)Maximum - (float)(object)Minimum) + (float)(object)Minimum), Maximum),
                    _ => Value
                };

                Callback?.Invoke(this);
            }
        }

        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            float percent = Value switch
            {
                int => ((int)(object)Value - (int)(object)Minimum) / (float)((int)(object)Maximum - (int)(object)Minimum),
                float => ((float)(object)Value - (float)(object)Minimum) / ((float)(object)Maximum - (float)(object)Minimum),
                _ => 0
            };

            Rectangle back = new((int)Position.X, (int)Position.Y, Width, Height);
            Rectangle front = new((int)(Position.X + percent * (Width - 40)), (int)Position.Y, 40, Height);

            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), back.X, back.Y, back.Width, back.Height, Color.White, Game1.pixelZoom, false);
            b.Draw(Game1.mouseCursors, new Vector2(front.X, front.Y), new Rectangle(420, 441, 10, 6), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
        }
    }
}
