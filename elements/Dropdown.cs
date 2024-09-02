using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SMUI.Elements.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class Dropdown : Element
    {
        public int DesiredWidth { get; set; }
        public int MaxOptionsInDrop { get; set; } = 5;
        public Texture2D? Texture { get; set; } = Game1.mouseCursors;
        public Rectangle BackgroundTextureRect { get; set; } = OptionsDropDown.dropDownBGSource;
        public Rectangle ButtonTextureRect { get; set; } = OptionsDropDown.dropDownButtonSource;

        public Option Option => Choices[ActiveChoice];
        public string Value
        {
            get => Choices[ActiveChoice].Value;
            set 
            {
                int index = Choices.FindIndex(x => x.Value == value);
                if (index != -1) ActiveChoice = index;
            }
        }
        public string Label => Choices[ActiveChoice].Label;

        public int ActiveChoice = 0;

        public int ActivePosition { get; set; } = 0;
        public List<Option> Choices { get; set; } = new();

        public bool Dropped;

        public Action<Dropdown>? OnChange;

        public static Dropdown? ActiveDropdown;
        public static int SinceDropdownWasActive = 0;

        /// <inheritdoc />
        public override int Width => Math.Max(300, Math.Min(500, DesiredWidth));

        /// <inheritdoc />
        public override int Height => 44;

        /// <inheritdoc />
        public override string ClickedSound { get; set; } = "shwip";

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            bool justClicked = false;
            if (Clicked && ActiveDropdown == null)
            {
                justClicked = true;
                Dropped = true;
                if(Parent != null)
                {
                    Parent.RenderLast = this;
                }
            }

            if (Dropped)
            {
                if (Constants.TargetPlatform != GamePlatform.Android)
                {
                    if ((Mouse.GetState().LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed ||
                         Game1.input.GetGamePadState().Buttons.A == ButtonState.Released && Game1.oldPadState.Buttons.A == ButtonState.Pressed)
                        && !justClicked)
                    {
                        Game1.playSound("drumkit6");
                        Dropped = false;
                        if (Parent?.RenderLast == this)
                            Parent.RenderLast = null;
                    }
                }
                else
                {
                    if ((Game1.input.GetMouseState().LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed ||
                         Game1.input.GetGamePadState().Buttons.A == ButtonState.Released && Game1.oldPadState.Buttons.A == ButtonState.Pressed)
                         && !justClicked)
                    {
                        Game1.playSound("drumkit6");
                        Dropped = false;
                        if (Parent?.RenderLast == this)
                            Parent.RenderLast = null;
                    }
                }

                int tall = Math.Min(MaxOptionsInDrop, Choices.Count - ActivePosition) * Height;
                int drawY = Math.Min((int)Position.Y, Game1.uiViewport.Height - tall);
                var bounds2 = new Rectangle((int)Position.X, drawY,Width, Height * MaxOptionsInDrop);
                if (bounds2.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
                {
                    int choice = (Game1.getOldMouseY() - drawY) / Height;
                    ActiveChoice = Math.Max(0, Math.Min(choice + ActivePosition, Choices.Count - 1));

                    OnChange?.Invoke(this);
                }
            }

            if (Dropped)
            {
                ActiveDropdown = this;
                SinceDropdownWasActive = 2; //Frame delay
            }
            else
            {
                if (ActiveDropdown == this)
                    ActiveDropdown = null;
                ActivePosition = Math.Min(ActiveChoice, Math.Max(Choices.Count - MaxOptionsInDrop, 0));
            }
        }

        public void ReceiveScrollWheelAction(int direction)
        {
            if (Dropped)
                ActivePosition = Math.Min(Math.Max(ActivePosition - (direction / 120), 0), Choices.Count - MaxOptionsInDrop);
            else
                ActiveDropdown = null;
        }

        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            IClickableMenu.drawTextureBox(b, Texture, BackgroundTextureRect, (int)Position.X, (int)Position.Y, Width - 48, Height, Color.White, 4, false);
            b.DrawString(Game1.smallFont, Label, new Vector2(Position.X + 4, Position.Y + 8), Game1.textColor);
            b.Draw(Texture, new Vector2(Position.X + Width - 48, Position.Y), ButtonTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);

            if (Dropped)
            {
                int maxValues = MaxOptionsInDrop;
                int start = ActivePosition;
                int end = Math.Min(Choices.Count, start + maxValues);
                int tall = Math.Min(maxValues, Choices.Count - ActivePosition) * Height;
                int drawY = Math.Min((int)Position.Y, Game1.uiViewport.Height - tall);
                IClickableMenu.drawTextureBox(b, Texture, BackgroundTextureRect, (int)Position.X, drawY, Width - 48, tall, Color.White, 4, false);
                for (int i = start; i < end; ++i)
                {
                    if (i == ActiveChoice)
                        b.Draw(Game1.staminaRect, new Rectangle((int)Position.X + 4, drawY + (i - ActivePosition) * Height, Width - 48 - 8, Height), null, Color.Wheat, 0, Vector2.Zero, SpriteEffects.None, 0.98f);
                    b.DrawString(Game1.smallFont, Choices[i].Label, new Vector2(Position.X + 4, drawY + (i - ActivePosition) * Height + 8), Game1.textColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                }
            }
        }
    }
}
