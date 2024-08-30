using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SMUI.Elements.Pickers
{
    public class DateTimePickerPopup : Container
    {
        public override int Width => 800;
        public override int Height => 500;

        public SDate Date => new(Day, Season, Year);
        public int Time { get; private set; } = 600;
        private int Day = Game1.dayOfMonth;
        private string Season = Game1.currentSeason;
        private int Year = Game1.year;

        private int m_ghostInterval = 0;
        public int GhostInterval
        {
            get => m_ghostInterval;
            set
            {
                UpdateHighlight();
            }
        }

        public bool Open { get; private set; }

        private const int ButtonWidth = 600;
        private const int ButtonHeight = 100;
        private static Vector2 EditorOffset => new(0, 108);

        public Action<Element>? OnChange;
        public Action<Element, bool>? OnToggle;

        //Closed UI
        private readonly Button popUpButton;
        private readonly Label popUpButtonLabel;

        //Open UI
        private readonly StaticContainer popUpBackground;
        private readonly Button closeButton;

        private const int DayPerRow = 7;
        private static Vector2 DaySelectorPosition => new(50, 100);
        private const int DayButtonWidth = 52;
        private const int DayButtonOffset = 4;
        private readonly List<Button> daySelectors;
        private readonly List<Label> daySelectorLabels;
        private readonly Intbox intervalInput;
        
        private readonly Dropdown seasonDropdown;
        public DateTimePickerPopup() : this(Vector2.Zero, Game1.timeOfDay, SDate.Now(), 0) { }

        public DateTimePickerPopup(Vector2 buttonSize, int time, SDate date, int interval)
        {
            Time = time;
            Day = date.Day;
            Season = date.Season.ToString();
            Year = date.Year;
            m_ghostInterval = interval;

            if(buttonSize == Vector2.Zero)
            {
                buttonSize = new(ButtonWidth, ButtonHeight);
            }
            Clickable = false;

            popUpButton = new(Game1.mouseCursors, new(384, 396, 15, 15))
            {
                Size = buttonSize,
                OnClick = (e) =>
                { 
                    Open = !Open;
                    popUpBackground!.Enabled = Open;
                }
            };
            AddChild(popUpButton);

            popUpButtonLabel = new();
            AddChild(popUpButtonLabel);
            
            //Open UI
            popUpBackground = new()
            {
                Size = new(Width, Height),
                OutlineColor = Color.Wheat,
                LocalPosition = EditorOffset
            };
            AddChild(popUpBackground);

            closeButton = new(Game1.mouseCursors, new Rectangle(337, 494, 12, 12))
            {
                Size = new(48, 48),
                OnClick = (e) =>
                {
                    Open = false;
                    popUpBackground!.Enabled = false;
                },
                LocalPosition = new Vector2(Width, -20)
            };
            popUpBackground.AddChild(closeButton);

            //Init date buttons
            daySelectors = new();
            daySelectorLabels = new();
            for (int i = 0; i < 28; i++)
            {
                int day = i + 1;
                int row = i / DayPerRow;
                int col = i % DayPerRow;

                //Button
                Vector2 buttonPos = new(
                    DaySelectorPosition.X + ((DayButtonWidth + DayButtonOffset) * col), 
                    DaySelectorPosition.Y + ((DayButtonWidth + DayButtonOffset) * row));
                daySelectors.Add(new(Game1.mouseCursors, new(432, 439, 9, 9))
                {
                    Size = new(DayButtonWidth, DayButtonWidth),
                    LocalPosition = buttonPos,
                    OnClick = (e) =>
                    {
                        Day = day;
                        UpdateHighlight();
                    }
                });
                popUpBackground.AddChild(daySelectors[i]);

                //Label
                string labelString = (i + 1).ToString();
                
                daySelectorLabels.Add(new()
                {
                    String = labelString,
                    Font = Game1.smallFont,
                    NonBoldShadow = false,
                });
                Vector2 labelPos = new(
                    buttonPos.X + ((DayButtonWidth - daySelectorLabels[i].Width) / 2),
                    buttonPos.Y + ((DayButtonWidth - daySelectorLabels[i].Height) / 2));
                daySelectorLabels[i].LocalPosition = labelPos;
                popUpBackground.AddChild(daySelectorLabels[i]);
            }

            //Day names for the calendar columns
            for (int i = 0; i < 7; i++)
            {
                Label name = new()
                {
                    Font = Game1.smallFont,
                    String = Utilities.NameOfDay(i + 1),
                };
                name.LocalPosition = new(
                        DaySelectorPosition.X + ((DayButtonWidth + DayButtonOffset) * i) + (DayButtonWidth - name.Width) / 2,
                        DaySelectorPosition.Y - 32);

                popUpBackground.AddChild(name);
            }

            intervalInput = new()
            {
                Value = GhostInterval,
                OnChange = (e) =>
                {
                    GhostInterval = (e as Intbox)!.Value;
                },
                LocalPosition = new(0, 0)
            };
            popUpBackground.AddChild(intervalInput);

            List<Option> options = new()
            {
                new(StardewValley.Season.Spring.ToString()),
                new(StardewValley.Season.Summer.ToString()),
                new(StardewValley.Season.Fall.ToString()),
                new(StardewValley.Season.Winter.ToString())
            };

            //Season dropdown
            seasonDropdown = new()
            {
                Choices = options,
                LocalPosition = new(500, 250),
                OnChange = (e) =>
                {
                    SetSeason(e.Value);
                }
            };
            popUpBackground.AddChild(seasonDropdown);

            UpdateDateLabel();
            UpdateHighlight();
        }

        public void UpdateHighlight()
        {
            daySelectors[Day - 1].IdleTint = Button.DefaultIdleTint; //Reset the previous day button colour
            daySelectors[Day - 1].HoverTint = Button.DefaultHoverTint;
            daySelectorLabels[Day - 1].Color = Game1.textColor; //Reset the previous day label colour

            if (GhostInterval > 0)
            {
                for (int i = 0; i < daySelectors.Count; i++)
                {
                    daySelectors[i].IdleTint = Button.DefaultIdleTint; //Reset the previous day button colour
                    daySelectors[i].HoverTint = Button.DefaultHoverTint;
                    daySelectorLabels[i].Color = Game1.textColor; //Reset the previous day label colour
                }
            }

            if (GhostInterval > 0)
            {
                for (int i = Day - 1; i < daySelectors.Count; i++)
                {
                    if ((i - (Day - 1)) % GhostInterval == 0)
                    {
                        daySelectors[i].IdleTint = Color.PaleGreen; //Highlight the current day button
                        daySelectors[i].HoverTint = Color.LightGreen;
                        daySelectorLabels[i].Color = Color.WhiteSmoke; //Change label to white for better contrast
                    }
                }
            }

            daySelectors[Day - 1].IdleTint = Color.LightSeaGreen; //Highlight the current day button
            daySelectors[Day - 1].HoverTint = Color.SeaGreen;
            daySelectorLabels[Day - 1].Color = Color.White; //Change label to white for better contrast
            OnChange?.Invoke(this);
        }

        public void SetSeason(string season)
        {
            Season = season;
            UpdateDateLabel();
            OnChange?.Invoke(this);
        }

        public override string ToString()
        {
            return $"{Date.ToLocaleString()} - {Time} {(Time < 1200 ? "am" : "pm")}";
        }

        private void UpdateDateLabel()
        {
            popUpButtonLabel.String = ToString();
            popUpButtonLabel.LocalPosition = new(
                (ButtonWidth - popUpButtonLabel.Width) / 2,
                (ButtonHeight - popUpButtonLabel.Height) / 2);
        }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);
        }

        public override void Draw(SpriteBatch b)
        {
            popUpButton.Draw(b);
            popUpButtonLabel.Draw(b);

            if (Open)
            {
                const int padding = 64;
                Rectangle contentArea = new(
                    (int)popUpBackground.Position.X - padding,
                    (int)popUpBackground.Position.Y - padding,
                    popUpBackground.Width + 2 * padding,
                    popUpBackground.Height + 2 * padding);

                Utilities.InScissorRectangle(b, contentArea, contentBatch =>
                {
                    popUpBackground.Draw(contentBatch);
                });
            }
        }
    }
}
