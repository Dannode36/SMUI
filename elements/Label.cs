using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace SMUI.Elements
{
    public class Label : Element
    {
        /*********
        ** Accessors
        *********/
        public bool Bold = false;
        public float NonBoldScale = 1f; // Only applies when Bold = false
        public bool NonBoldShadow = true; // Only applies when Bold = false
        public Color Color = Game1.textColor;

        public SpriteFont Font = Game1.dialogueFont; // Only applies when Bold = false

        public float Scale => Bold ? 1f : NonBoldScale;

        public string String = string.Empty;

        /// <inheritdoc />
        public override int Width => (int)Measure().X;

        /// <inheritdoc />
        public override int Height => (int)Measure().Y;

        /// <inheritdoc />
        public override string HoveredSound { get; set; } = "shiny4";
        public override bool Clickable { get; set; } = false;

        public Label() { }
        public Label(string str, float scale, bool bold = false, SpriteFont? font = null)
        {
            String = str;
            NonBoldScale = scale;
            Bold = bold;
            if(font == null) Font = Game1.dialogueFont;
        }

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

        /// <summary>Measure the label's rendered dialogue text size.</summary>
        public Vector2 Measure()
        {
            return MeasureString(String, Bold, scale: Bold ? 1f : NonBoldScale, font: Font);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            bool altColor = Hover && OnClick != null;
            if (Bold)
            {
                SpriteText.drawString(b, String, (int)Position.X, (int)Position.Y, layerDepth: 1, color: altColor ? SpriteText.color_Gray : null);
            }
            else
            {
                if (Color.A <= 0)
                    return;

                if (NonBoldShadow)
                    Utility.drawTextWithShadow(b, String, Font, Position, Color, NonBoldScale);
                else
                    b.DrawString(Font, String, Position, Color, 0f, Vector2.Zero, NonBoldScale, SpriteEffects.None, 1);
            }
        }

        /// <summary>Measure the rendered dialogue text size for the given text.</summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="bold">Whether the font is bold.</param>
        /// <param name="scale">The scale to apply to the size.</param>
        /// <param name="font">The font to measure. Defaults to <see cref="Game1.dialogueFont"/> if <c>null</c>.</param>
        public static Vector2 MeasureString(string text, bool bold = false, float scale = 1f, SpriteFont? font = null)
        {
            if (bold)
                return new Vector2(SpriteText.getWidthOfString(text) * scale, SpriteText.getHeightOfString(text) * scale);
            else
                return (font ?? Game1.dialogueFont).MeasureString(text) * scale;
        }
    }
}
