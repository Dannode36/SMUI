using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class Button : Element, ISingleTexture
    {
        public static Color IdleTintColour => Color.White;
        public static Color HoverTintColour => Color.Wheat;

        public Texture2D? Texture { get; set; }
        public Rectangle TextureRect { get; set; } = Rectangle.Empty;

        public Color IdleTint { get; set; }
        public Color HoverTint { get; set; }

        public Action<Element>? Callback { get; set; }

        /// <summary>Size of button when using box draw</summary>
        public Vector2 Size { get; set; }

        /// <summary>When true draws the texture to fit a box. When false draws the texture rect as is</summary>
        public bool BoxDraw {  get; set; } = true;

        public int Scale { get; set; } = Game1.pixelZoom;

        /// <inheritdoc />
        public override int Width => BoxDraw ? (int)Size.X : TextureRect.Width * Scale;

        /// <inheritdoc />
        public override int Height => BoxDraw ? (int)Size.Y : TextureRect.Height * Scale;

        /// <inheritdoc />
        public override string HoveredSound => "Cowboy_Footstep";

        /*********
        ** Public methods
        *********/
        public Button() { }

        public Button(Texture2D tex, Rectangle rect)
        {
            Texture = tex;
            TextureRect = rect;
            IdleTint = Color.White;
            HoverTint = Color.Wheat;
        }

        public Button(Texture2D tex, Rectangle rect, Color idleTint, Color hoverTint)
        {
            Texture = tex;
            TextureRect = rect;
            IdleTint = idleTint;
            HoverTint = hoverTint;
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            if (Clicked)
                Callback?.Invoke(this);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            if (BoxDraw)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, TextureRect, (int)Position.X, (int)Position.Y, Width, Height, Hover ? HoverTint : IdleTint, Scale, drawShadow: false);
            }
            else
            {
                b.Draw(Texture, Position, TextureRect, Hover ? HoverTint : IdleTint, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
            }
        }
    }
}
