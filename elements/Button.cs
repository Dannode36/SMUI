using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ValleyReminders.ui
{
    internal class Button : Element, ISingleTexture
    {
        /*********
        ** Fields
        *********/
        private float Scale = 1f;

        /*********
        ** Accessors
        *********/
        public Texture2D? Texture { get; set; }
        public Rectangle TextureRect { get; set; } = Rectangle.Empty;

        public Color IdleTint { get; set; }
        public Color HoverTint { get; set; }

        public Action<Element>? Callback { get; set; }

        public Vector2 Size { get; set; }

        /// <inheritdoc />
        public override int Width => (int)Size.X;

        /// <inheritdoc />
        public override int Height => (int)Size.Y;

        /// <inheritdoc />
        public override string HoveredSound => "Cowboy_Footstep";

        /*********
        ** Public methods
        *********/
        public Button() { }

        public Button(Texture2D tex, Rectangle rect, Vector2 size)
        {
            Texture = tex;
            TextureRect = rect;
            Size = size;
            IdleTint = Color.White;
            HoverTint = Color.Wheat;
        }

        public Button(Texture2D tex, Rectangle rect, Vector2 size, Color idleTint, Color hoverTint)
        {
            Texture = tex;
            TextureRect = rect;
            Size = size;
            IdleTint = idleTint;
            HoverTint = hoverTint;
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            Scale = Hover ? Math.Min(Scale + 0.013f, 1.083f) : Math.Max(Scale - 0.013f, 1f);

            if (Clicked)
                Callback?.Invoke(this);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, TextureRect, (int)Position.X, (int)Position.Y, Width, Height, Hover ? HoverTint : IdleTint, 4f, drawShadow: false);

            //Game1.activeClickableMenu?.drawMouse(b);
        }
    }
}
