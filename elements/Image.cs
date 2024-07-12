using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SMUI.Elements
{
    public class Image : Element, ISingleTexture
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The image texture to display.</summary>
        public Texture2D? Texture { get; set; }

        /// <summary>The pixel area within the texture to display, or <c>null</c> to show the entire image.</summary>
        public Rectangle? TextureArea { get; set; }

        /// <summary>The zoom factor to apply to the image.</summary>
        public float Scale { get; set; } = Game1.pixelZoom;

        public override bool Clickable { get; set; } = false;
        public Action<Image>? OnClick { get; set; }

        /// <inheritdoc />
        public override int Width => (int)GetActualSize().X;

        /// <inheritdoc />
        public override int Height => (int)GetActualSize().Y;

        /// <inheritdoc />
        public override string HoveredSound => (Clickable) ? "shiny4" : string.Empty;

        public Color DrawColor { get; set; } = Color.White;

        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            if (Clicked)
                OnClick?.Invoke(this);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            b.Draw(Texture, Position, TextureArea, DrawColor, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
        }

        /*********
        ** Private methods
        *********/
        private Vector2 GetActualSize()
        {
            if (TextureArea.HasValue)
                return new Vector2(TextureArea.Value.Width, TextureArea.Value.Height) * Scale;
            else
                return Texture == null ? Vector2.Zero : new Vector2(Texture.Width, Texture.Height) * Scale;
        }
    }
}
