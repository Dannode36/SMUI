using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class Checkbox : Element
    {
        /*********
        ** Accessors
        *********/
        public Texture2D? Texture;
        public Rectangle CheckedTextureRect;
        public Rectangle UncheckedTextureRect;

        public bool Checked { get; set; } = true;

        /// <inheritdoc />
        public override int Width => UncheckedTextureRect.Width * 4;

        /// <inheritdoc />
        public override int Height => UncheckedTextureRect.Height * 4;

        /// <inheritdoc />
        public override string ClickedSound { get; set; } = "drumkit6";

        /*********
        ** Public methods
        *********/
        public Checkbox()
        {
            Texture = Game1.mouseCursors;
            CheckedTextureRect = OptionsCheckbox.sourceRectChecked;
            UncheckedTextureRect = OptionsCheckbox.sourceRectUnchecked;
        }

        public Checkbox(Texture2D tex, Rectangle checkedRect, Rectangle uncheckedRect)
        {
            Texture = tex;
            CheckedTextureRect = checkedRect;
            UncheckedTextureRect = uncheckedRect;
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            if (Clicked && OnClick != null)
            {
                Checked = !Checked;
            }
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            b.Draw(Texture, Position, Checked ? CheckedTextureRect : UncheckedTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
            Game1.activeClickableMenu?.drawMouse(b);
        }
    }
}
