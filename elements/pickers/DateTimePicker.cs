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
        public override int Width => 800;
        public override int Height => 500;

        public SDate Date => new(Day, Season, Year);
        public int Time { get; private set; } = 600;
        private int Day = Game1.dayOfMonth;
        private string Season = Game1.currentSeason;
        private int Year = Game1.year;

        public bool Open { get; private set; }

        private const int ButtonWidth = 600;
        private const int ButtonHeight = 100;
        private static Vector2 EditorOffset => new(50, 100);

        public Action<Element>? Callback { get; set; }

        //Closed UI
        private readonly Button button;
        private readonly Label label;
        private bool dateLabelDirty = false;

        //Open UI
        private readonly Button closeButton;
        private readonly StaticContainer background;

        private const int DayPerRow = 7;
        private static Vector2 DaySelectorPosition => new(50, 100);
        private const int DayButtonWidth = 50;
        private readonly List<Button> daySelectors;
        private readonly List<Label> daySelectorLabels;

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
                String = $"{Date.ToLocaleString()} @ {Time}",
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
                LocalPosition = new Vector2(Width, -12) + EditorOffset
            };
            AddChild(closeButton);

            background = new()
            {
                Size = new(Width, Height),
                OutlineColor = Color.Wheat,
                LocalPosition = EditorOffset
            };
            AddChild(background);

            //Init date buttons
            daySelectors = new();
            daySelectorLabels = new();
            for (int i = 0; i < 28; i++)
            {
                int day = i + 1;
                int row = i / DayPerRow;
                int col = i % DayPerRow;
                const int offset = 4;

                Vector2 buttonPos = new(DaySelectorPosition.X + ((DayButtonWidth + offset) * col), DaySelectorPosition.Y + ((DayButtonWidth + offset) * row));

                daySelectors.Add(new(Game1.mouseCursors, new(432, 439, 9, 9), new(DayButtonWidth, DayButtonWidth))
                {
                    LocalPosition = buttonPos,
                    Callback = (e) =>
                    {
                        Day = day;
                        dateLabelDirty = true;
                        Callback?.Invoke(this); //DateTimePicker callback
                    }
                });

                string labelString = (i + 1).ToString();
                Vector2 labelPos = new(
                    buttonPos.X + ((DayButtonWidth - Label.MeasureString(labelString, font: Game1.smallFont).X) / 2), 
                    buttonPos.Y + ((DayButtonWidth - Label.MeasureString(labelString, font: Game1.smallFont).Y) / 2));

                daySelectorLabels.Add(new()
                {
                    LocalPosition = labelPos,
                    String = labelString,
                    Font = Game1.smallFont
                });

                AddChild(daySelectors[i]);
                AddChild(daySelectorLabels[i]);
            }
        }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            if(dateLabelDirty)
            {
                label.String = $"{Date.ToLocaleString()} @ {Time}";
                label.LocalPosition = new(
                    (ButtonWidth - Label.MeasureString(label.String).X) / 2,
                    (ButtonHeight - Label.MeasureString(label.String).Y) / 2);
                dateLabelDirty = false;
            }
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

                    foreach (var daySelector in daySelectors)
                    {
                        daySelector.Draw(contentBatch);
                    }

                    foreach (var daySelectorLabel in daySelectorLabels)
                    {
                        daySelectorLabel.Draw(contentBatch);
                    }

                    closeButton.Draw(contentBatch);
                });
            }
        }
    }
}
