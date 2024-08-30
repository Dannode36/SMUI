using System;
using System.Reflection.Emit;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class Button : Element
    {
        public static Color DefaultIdleTint => Color.White;
        public static Color DefaultHoverTint => Color.Wheat;

        public Texture2D? Texture { get; set; }
        public Rectangle TextureRect = Rectangle.Empty;

        public Color IdleTint;
        public Color HoverTint;

        /// <summary>Size of button when using box draw</summary>
        public Vector2 Size;

        /// <summary>When true draws the texture to fit a box. When false draws the texture rect as is</summary>
        public bool BoxDraw = false;

        public float Scale = Game1.pixelZoom;
        public float HoverScale = Game1.pixelZoom + 0.22f;
        public float ScaleSpeed = 0.034f;
        public float m_trueScale { get; private set; }

        public override int Width => BoxDraw ? (int)Size.X : TextureRect.Width * (int)Scale;

        public override int Height => BoxDraw ? (int)Size.Y : TextureRect.Height * (int)Scale;

        public override string HoveredSound { get; set; } = "Cowboy_Footstep";

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
            m_trueScale = Scale;
        }

        public Button(Texture2D tex, Rectangle rect, Color idleTint, Color hoverTint)
        {
            Texture = tex;
            TextureRect = rect;
            IdleTint = idleTint;
            HoverTint = hoverTint;
            m_trueScale = Scale;
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            m_trueScale = Hover ? Math.Min(m_trueScale + ScaleSpeed, HoverScale) : Math.Max(m_trueScale - ScaleSpeed, Scale);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            if (BoxDraw)
            {
                IClickableMenu.drawTextureBox(b, Texture, TextureRect, (int)Position.X, (int)Position.Y, Width, Height, Hover ? HoverTint : IdleTint, Scale, drawShadow: false);
            }
            else
            {
                b.Draw(Texture, Position + (new Vector2(Width, Height) * 0.5f), TextureRect, Hover ? HoverTint : IdleTint, 0, TextureRect.Size.ToVector2() / 2f, m_trueScale, SpriteEffects.None, 1);
            }

            if (Hover && !string.IsNullOrEmpty(Tooltip))
            {
                IClickableMenu.drawHoverText(b, Tooltip, Game1.smallFont);
            }
        }
    }
}
