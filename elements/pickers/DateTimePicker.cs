using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SMUI.Elements.Pickers
{
    public class DateTimePicker : Container
    {
        

        public override int Width => 500;
        public override int Height => 300;

        public SDate SDate { get; private set; } = SDate.Now();
        public int Time { get; private set; } = 600;

        public bool Open { get; private set; }

        private const int ButtonWidth = 600;
        private const int ButtonHeight = 100;
        private Vector2 editorOffset => new(50, 100);

        //Closed UI
        private readonly Button button;
        private readonly Label label;

        //Open UI
        private readonly Button closeButton;
        private readonly StaticContainer background;

        public DateTimePicker()
        {
            Clickable = false;

            button = new(Game1.mouseCursors, new(384, 396, 15, 15), new(ButtonWidth, ButtonHeight))
            {
                Callback = (e) =>
                { 
                    Open = true;
                    background!.Enabled = true;
                    closeButton!.Enabled = true;
                }
            };
            AddChild(button);

            label = new()
            {
                //String = $"{SDate.ToLocaleString()} @ {Time}",
                String = "Summer 28 in year 10 @ 2600",
            };
            label.LocalPosition = new(
                (ButtonWidth - Label.MeasureString(label.String).X) / 2,
                (ButtonHeight - Label.MeasureString(label.String).Y) / 2);

            AddChild(label);

            closeButton = new(Game1.mouseCursors, new Rectangle(337, 494, 12, 12), new(48, 48))
            {
                Callback = (e) =>
                {
                    Open = false;
                    background!.Enabled = false;
                    closeButton!.Enabled = false;
                },
                LocalPosition = new Vector2(500, -12) + editorOffset
            };
            AddChild(closeButton);

            background = new()
            {
                Size = new(Width, Height),
                OutlineColor = Color.Wheat,
                LocalPosition = editorOffset
            };
            AddChild(background);
        }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);
        }

        public override void Draw(SpriteBatch b)
        {
            button.Draw(b);
            label.Draw(b);

            if (Open)
            {
                const int padding = 64;
                Rectangle contentArea = new(
                    (int)background.Position.X - padding,
                    (int)background.Position.Y - padding,
                    (int)background.Width + 2 * padding,
                    (int)background.Height + 2 * padding);

                Utilities.InScissorRectangle(b, contentArea, contentBatch =>
                {
                    background.Draw(contentBatch);
                    closeButton.Draw(contentBatch);
                });
            }
        }
    }
}
