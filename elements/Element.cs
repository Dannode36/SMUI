using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SMUI.Types;
using StardewModdingAPI;
using StardewValley;
using static StardewValley.LocationRequest;

namespace SMUI.Elements
{
    public abstract class Element
    {
        /*********
        ** Accessors
        *********/
        public object? UserData { get; set; }
        public string UUID { get; set; } = string.Empty;

        public Container? Parent { get; internal set; }
        public Vector2 LocalPosition;
        public Vector2 Position
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Position + LocalPosition;
                }
                return LocalPosition;
            }
        }

        public Action<Element>? OnClick;
        public Action<Element>? OnHover;

        public abstract int Width { get; }
        public abstract int Height { get; }
        public Margin Margin { get; set; }
        public Rectangle Bounds => new((int)Position.X - Margin.Left, (int)Position.Y - Margin.Top, Width + Margin.Left + Margin.Right, Height + Margin.Top + Margin.Bottom);

        public bool Hover { get; private set; }
        public virtual string HoveredSound { get; set; } = string.Empty;
        public virtual string Tooltip { get; set; } = string.Empty;

        public virtual bool Clickable { get; set; } = true;
        public bool ClickGestured { get; private set; }
        public bool Clicked => Hover && ClickGestured;
        public virtual string ClickedSound { get; set; } = string.Empty;

        /// <summary>Whether to disable the element so it's invisible and can't be interacted with.</summary>
        public Func<bool>? HideCondition;

        public bool Enabled { get; set; } = true;

        /*********
        ** Public methods
        *********/
        /// <summary>Update the element for the current game tick.</summary>
        /// <param name="isOffScreen">Whether the element is currently off-screen.</param>
        public virtual void Update(bool isOffScreen = false)
        {
            bool hidden = IsHidden(isOffScreen);

            if (hidden || !Enabled)
            {
                Hover = false;
                ClickGestured = false;
                return;
            }

            float mouseX;
            float mouseY;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                mouseX = Game1.getMouseX() / Game1.options.uiScale;
                mouseY = Game1.getMouseY() / Game1.options.uiScale;
            }
            else
            {
                mouseX = Game1.getOldMouseX() / Game1.options.uiScale;
                mouseY = Game1.getOldMouseY() / Game1.options.uiScale;
            }

            bool newHover = !hidden && !GetRoot().Obscured && Bounds.Contains(mouseX, mouseY);
            if (newHover && !Hover)
            {
                if(HoveredSound != string.Empty)
                {
                    Game1.playSound(HoveredSound);
                }
                OnHover?.Invoke(this);
            }
            Hover = newHover;

            if(Clickable && Parent != null && !Parent.ClickConsumed)
            {
                ClickGestured = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released;
                ClickGestured = ClickGestured || (Game1.options.gamepadControls && (Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)));
                
                if (Clicked)
                {
                    Parent.ClickConsumed = true;
                    Parent.ClickConsumer = this;

                    if (Dropdown.SinceDropdownWasActive > 0 || Dropdown.ActiveDropdown != null)
                    {
                        ClickGestured = false;
                    }

                    if (ClickedSound != string.Empty)
                    {
                        Game1.playSound(ClickedSound);
                    }
                    OnClick?.Invoke(this);
                }
            }
            else //Cover any edge cases
            {
                ClickGestured = false;
            }
        }

        public abstract void Draw(SpriteBatch b);

        public Root GetRoot()
        {
            return GetRootImpl();
        }

        internal virtual Root GetRootImpl()
        {
            if (Parent == null)
                throw new Exception("Element must have a parent.");
            return Parent.GetRoot();
        }

        /// <summary>Get whether the element is hidden based on <see cref="HideCondition"/> or its position relative to the screen.</summary>
        /// <param name="isOffScreen">Whether the element is currently off-screen.</param>
        public bool IsHidden(bool isOffScreen = false)
        {
            return isOffScreen || HideCondition?.Invoke() == true || !Enabled;
        }
    }
}
