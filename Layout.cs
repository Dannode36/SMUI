using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sickhead.Engine.Util;
using SMUI.Elements.Data;
using SMUI.Elements;
using SMUI.Elements.Pickers;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static StardewValley.Objects.BedFurniture;

namespace SMUI.Layout
{
    public static class LayoutHelper
    {
        public static IModHelper helper;
        public static IMonitor monitor;

        public static List<Layout> Layouts = new();
        static private Dictionary<string, string> layoutNameToPath = new();

        public static Dictionary<string, object> EventRegistry = new();

        public static bool AddEventHandler(string name, Action<Element> handler)
        {
            return EventRegistry.TryAdd(name, handler);
        }
        public static void RemoveEventHandler(string name)
        {
            EventRegistry.Remove(name);
        }

        public static Layout LoadLayout(string name)
        {
            var stream = new StreamReader(File.OpenRead(helper.DirectoryPath + "\\" + name));
            
            XDocument xmlDoc = XDocument.Load(stream, LoadOptions.SetLineInfo);
            Layout layout = new();

            if (layout.Parse(xmlDoc))
            {
                Layouts.Add(layout);
            }
            else
            {
                monitor.Log("Invalid XML layout", LogLevel.Error);
            }
            return layout;
        }

        public static Rectangle ToRect(this Vector4 vector)
        {
            return new((int)vector.X, (int)vector.Y, (int)vector.Z, (int)vector.W);
        }

        public static int GetElementLine(XElement xElement)
        {
            return ((IXmlLineInfo)xElement).HasLineInfo() ? ((IXmlLineInfo)xElement).LineNumber : -1;
        }
    }

    public class Layout
    {
        public string rootPath = string.Empty;
        public RootElement root = new();
        public Dictionary<string, Element> uuidElements = new();

        private const string attr_OnClick = "onClick";
        private const string attr_OnHover = "onHover";
        private const string attr_OnChange = "onChange";
        private const string attr_UUID = "uuid";
        private const string attr_Postion = "pos";
        private const string attr_HoverSound = "hoverSound";
        private const string attr_ClickSound = "clickSound";
        private const string attr_Tooltip = "tooltip";
        private const string attr_Enabled = "enabled";
        private const string attr_Clickable = "clickable";
        private const string attr_Texture = "tex";
        private const string attr_TextureRect = "texRect";
        private const string attr_CheckedTextureRect = "checkedTexRect";
        private const string attr_UncheckedTextureRect = "uncheckedTexRect";
        private const string attr_Size = "size";
        private const string attr_Scale = "scale";
        private const string attr_HoverScale = "hoverScale";
        private const string attr_ScaleSpeed = "scaleSpeed";
        private const string attr_BoxDraw = "boxDraw";
        private const string attr_Tint = "tint";
        private const string attr_IdleTint = "idleTint";
        private const string attr_HoverTint = "hoverTint";
        private const string attr_OutlineColor = "shadow";
        private const string attr_SelectedChoice = "choice";
        private const string attr_Font = "font";
        private const string attr_Bold = "bold";
        private const string attr_Shadow = "shadow";
        private const string attr_Value = "shadow";
        private const string attr_Min = "shadow";
        private const string attr_Max = "shadow";
        private const string attr_Interval = "shadow";

        public bool Parse(XDocument xml)
        {
            if (xml.Root == null)
            {
                return false;
            }
            foreach (var element in xml.Root.Elements())
            {
                root.AddChild(ParseGenericElement(element));
            }
            return true;
        }
        private Element ParseGenericElement(XElement xElement)
        {
            Element element = ParseElementTag(xElement);

            element.UUID = xElement.Attribute(attr_UUID)?.Value ?? element.UUID;
            if (!string.IsNullOrEmpty(element.UUID))
            {
                uuidElements.Add(element.UUID, element);
            }

            TryVector2FromAttribute(xElement.Attribute(attr_Postion), ref element.LocalPosition);

            element.HoveredSound = xElement.Attribute(attr_HoverSound)?.Value ?? element.HoveredSound;
            element.ClickedSound = xElement.Attribute(attr_ClickSound)?.Value ?? element.ClickedSound;
            element.Tooltip = xElement.Attribute(attr_Tooltip)?.Value ?? element.Tooltip;

            TryBindEventHandler(xElement, attr_OnClick, ref element.OnClick);
            TryBindEventHandler(xElement, attr_OnHover, ref element.OnHover);

            var enabledAttr = xElement.Attribute(attr_Enabled);
            if (enabledAttr != null)
            {
                element.Enabled = bool.Parse(enabledAttr.Value);
            }

            var clickableAttr = xElement.Attribute(attr_Clickable);
            if (clickableAttr != null)
            {
                element.Clickable = bool.Parse(clickableAttr.Value);
            }

            return element;
        }
        private Element ParseElementTag(XElement xElement)
        {
            switch (xElement.Name.LocalName)
            {
                case nameof(Button):
                    var btn_textureAttr = xElement.Attribute(attr_Texture);
                    var btn_rectAttr = xElement.Attribute(attr_TextureRect) ?? throw new($"Button tag (line {LayoutHelper.GetElementLine(xElement)}) did not specify a rect");
                    Button button = new(GetBestTexture(btn_textureAttr.Value ?? "mouseCursors"), ParseVector4(btn_rectAttr.Value).ToRect());

                    //OPTIONALS
                    TryFloatFromAttribute(xElement.Attribute(attr_Scale), ref button.Scale);
                    TryFloatFromAttribute(xElement.Attribute(attr_HoverScale), ref button.HoverScale);
                    TryFloatFromAttribute(xElement.Attribute(attr_ScaleSpeed), ref button.ScaleSpeed);
                    TryBoolFromAttribute(xElement.Attribute(attr_BoxDraw), ref button.BoxDraw);
                    TryColorFromAttribute(xElement.Attribute(attr_IdleTint), ref button.IdleTint);
                    TryColorFromAttribute(xElement.Attribute(attr_HoverTint), ref button.HoverTint);
                    TryVector2FromAttribute(xElement.Attribute(attr_Size), ref button.Size);
                    return button;
                case nameof(Checkbox):
                    Checkbox checkbox = new();

                    var cbx_textureAttr = xElement.Attribute(attr_Texture);
                    if (cbx_textureAttr != null)
                    {
                        checkbox.Texture = GetBestTexture(cbx_textureAttr.Value);
                    }
                    TryRectFromAttribute(xElement.Attribute(attr_CheckedTextureRect), ref checkbox.CheckedTextureRect);
                    TryRectFromAttribute(xElement.Attribute(attr_UncheckedTextureRect), ref checkbox.UncheckedTextureRect);

                    return checkbox;
                case nameof(Dropdown):
                    Dropdown dropdown = new();
                    foreach (var child in xElement.Elements())
                    {
                        if (child.Name == nameof(Option))
                        {
                            dropdown.Choices.Add(ParseOption(child));
                        }
                        else
                        {
                            throw new($"Child of Dropdown (line {LayoutHelper.GetElementLine(xElement)}) was not an Option tag");
                        }
                    }

                    TryIntFromAttribute(xElement.Attribute(attr_SelectedChoice), ref dropdown.ActiveChoice);
                    TryBindEventHandler(xElement, attr_OnChange, ref dropdown.OnChange);
                    return dropdown;
                case nameof(Floatbox):
                    Floatbox floatbox = new();
                    if(float.TryParse(xElement.Value, out float floatValue))
                    {
                        floatbox.Value = floatValue;
                    }
                    return floatbox;
                case nameof(Image):
                    Image image = new();

                    var image_textureAttr = xElement.Attribute(attr_Texture);
                    if (image_textureAttr != null)
                    {
                        image.Texture = GetBestTexture(image_textureAttr.Value);
                    }
                    TryRectFromAttribute(xElement.Attribute(attr_TextureRect), ref image.TextureArea);
                    TryFloatFromAttribute(xElement.Attribute(attr_Scale), ref image.Scale);
                    TryColorFromAttribute(xElement.Attribute(attr_Tint), ref image.DrawColor);
                    return image;
                case nameof(Intbox):
                    Intbox intbox = new();
                    if (int.TryParse(xElement.Value, out int intValue))
                    {
                        intbox.Value = intValue;
                    }
                    return intbox;
                case nameof(ItemSlot):
                    return new ItemSlot(); //Unsupported
                case nameof(ItemWithBorder):
                    return new ItemWithBorder(); //Unsupported
                case nameof(Label):
                    Label label = new()
                    {
                        String = xElement.Value,
                        Font = GetBestFont(xElement.Attribute(attr_Font)?.Value ?? "SmallFont")
                    };

                    TryBoolFromAttribute(xElement.Attribute(attr_Bold), ref label.Bold);
                    TryBoolFromAttribute(xElement.Attribute(attr_Shadow), ref label.NonBoldShadow);
                    TryFloatFromAttribute(xElement.Attribute(attr_Scale), ref label.NonBoldScale);
                    TryColorFromAttribute(xElement.Attribute(attr_Tint), ref label.Color);
                    return label;
                case nameof(Row):
                    Row row = new();

                    foreach (var element in xElement.Elements())
                    {
                        row.AddChild(ParseGenericElement(element));
                    }
                    return row;
                case nameof(Scrollbar):
                    return new Scrollbar(); //Unsupported as of yet
                case nameof(Slider):
                    Slider<float> slider = new();
                    TryVector2FromAttribute(xElement.Attribute(attr_Size), ref slider.Size);
                    TryBindEventHandler(xElement, attr_OnChange, ref slider.OnChange);

                    TryFloatFromAttribute(xElement.Attribute(attr_Min), ref slider.Minimum);
                    TryFloatFromAttribute(xElement.Attribute(attr_Max), ref slider.Maximum);
                    TryFloatFromAttribute(xElement.Attribute(attr_Value), ref slider.Value);
                    TryFloatFromAttribute(xElement.Attribute(attr_Interval), ref slider.Interval);

                    return slider;
                case nameof(StaticContainer):
                    StaticContainer staticContainer = new();

                    TryColorFromAttribute(xElement.Attribute(attr_OutlineColor), ref staticContainer.OutlineColor);
                    TryVector2FromAttribute(xElement.Attribute(attr_Size), ref staticContainer.Size);

                    foreach (var element in xElement.Elements())
                    {
                        staticContainer.AddChild(ParseGenericElement(element));
                    }
                    return staticContainer;
                case nameof(Table):
                    Table table = new();

                    foreach (var tableChild in xElement.Elements())
                    {
                        if(tableChild.Name == nameof(Row))
                        {
                            Row rowElement = new();

                            foreach (var element in tableChild.Elements())
                            {
                                rowElement.AddChild(ParseGenericElement(element));
                            }

                            table.AddChild(rowElement);
                        }
                        else
                        {
                            Row rowElement = new(new[] { ParseGenericElement(tableChild) });
                            table.AddChild(rowElement);
                        }
                    }
                    return table;
                case nameof(Textbox):
                    Textbox textbox = new()
                    {
                        String = xElement.Value
                    };
                    TryBindEventHandler(xElement, attr_OnChange, ref textbox.OnChange);
                    return textbox;
                case nameof(DateTimePicker):
                    return new DateTimePicker();
                case nameof(DateTimePickerPopup):
                    return new DateTimePickerPopup();
                default:
                    throw new($"Invalid element tag (line {LayoutHelper.GetElementLine(xElement)})");
            }
        }

        private static Option ParseOption(XElement xElement)
        {
            var valueAttr = xElement.Attribute(attr_Value);
            if (valueAttr != null)
            {
                if (string.IsNullOrWhiteSpace(xElement.Value))
                {
                    return new(valueAttr.Value);
                }
                return new Option(xElement.Value, valueAttr.Value);
            }

            return new Option();
        }
        private Texture2D? GetBestTexture(string name)
        {
            var game1Type = typeof(Game1);
            var game1Prop = game1Type.GetMember(name);
            if (game1Prop != null)
            {
                var gameTex = (Texture2D?)(game1Prop?.FirstOrDefault()?.GetValue(null));

                if(gameTex != null)
                {
                    return gameTex;
                }
            }

            var gameContentTex = LayoutHelper.helper.GameContent.Load<Texture2D>("LooseSprites\\" + name);
            if(gameContentTex != null)
            {
                return gameContentTex;
            }

            var modTexture = LayoutHelper.helper.ModContent.Load<Texture2D>(rootPath + "/" + name);
            if (modTexture != null)
            {
                return modTexture;
            }

            LayoutHelper.monitor.Log($"No texture with the name \"{name}\" could be found", LogLevel.Error);
            return null;
        }
        private SpriteFont? GetBestFont(string name)
        {
            var game1Type = typeof(Game1);
            var game1Prop = game1Type.GetMember(name);
            if (game1Prop != null)
            {
                var gameFont = (SpriteFont?)(game1Prop?.FirstOrDefault()?.GetValue(null));

                if(gameFont != null)
                {
                    return gameFont;
                }
            }

            var gameContentFont = LayoutHelper.helper.GameContent.Load<SpriteFont>("Fonts\\" + name);
            if(gameContentFont != null)
            {
                return gameContentFont;
            }

            var modFont = LayoutHelper.helper.ModContent.Load<SpriteFont>(rootPath + "/" + name);
            if (modFont != null)
            {
                return modFont;
            }

            LayoutHelper.monitor.Log($"No font with the name \"{name}\" could be found", LogLevel.Error);
            return null;
        }

        private static Vector2 ParseVector2(string definition)
        {
            float[] values = Array.ConvertAll(definition.Split(','), s => float.Parse(s));

            if (values.Length < 2)
            {
                LayoutHelper.monitor.Log("Invalid rectangle attribute", LogLevel.Error);
                return Vector2.Zero;
            }

            return new(values[0], values[1]);
        }
        private static Vector3 ParseVector3(string definition)
        {
            float[] values = Array.ConvertAll(definition.Split(','), s => float.Parse(s));

            if (values.Length < 3)
            {
                LayoutHelper.monitor.Log("Invalid rectangle attribute", LogLevel.Error);
                return Vector3.Zero;
            }

            return new(values[0], values[1], values[2]);
        }
        private static Vector4 ParseVector4(string definition)
        {
            float[] values = Array.ConvertAll(definition.Split(','), s => float.Parse(s));

            if (values.Length < 4)
            {
                LayoutHelper.monitor.Log("Invalid rectangle attribute", LogLevel.Error);
                return Vector4.Zero;
            }

            return new(values[0], values[1], values[2], values[3]);
        }

        private static bool TryBindEventHandler<T>(XElement xElement, string name, ref Action<T>? handler)
        {
            var handlerAttr = xElement.Attribute(name);
            if (handlerAttr != null)
            {
                if (LayoutHelper.EventRegistry.TryGetValue(handlerAttr.Value, out var clickAction))
                {
                    handler = (Action<T>?)clickAction;
                    return true;
                }
                else
                {
                    LayoutHelper.monitor.Log($"No event handler named, \"{handlerAttr.Value}\" is registered. " +
                        $"Either register a handler under that name and call parse again or bind one manually", LogLevel.Error);
                }
            }
            return false;
        }
        private static bool TryIntFromAttribute(XAttribute? attribute, ref int value)
        {
            if (attribute != null)
            {
                value = int.Parse(attribute.Value);
                return true;
            }
            return false;
        }
        private static bool TryFloatFromAttribute(XAttribute? attribute, ref float value)
        {
            if (attribute != null)
            {
                value = float.Parse(attribute.Value);
                return true;
            }
            return false;
        }
        private static bool TryBoolFromAttribute(XAttribute? attribute, ref bool value)
        {
            if (attribute != null)
            {
                value = bool.Parse(attribute.Value);
                return true;
            }
            return false;
        }
        private static bool TryVector2FromAttribute(XAttribute? attribute, ref Vector2 value)
        {
            if (attribute != null)
            {
                value = ParseVector2(attribute.Value);
                return true;
            }
            return false;
        }
        private static bool TryVector4FromAttribute(XAttribute? attribute, ref Vector4 value)
        {
            if (attribute != null)
            {
                value = ParseVector4(attribute.Value);
                return true;
            }
            return false;
        }
        private static bool TryColorFromAttribute(XAttribute? attribute, ref Color value)
        {
            if (attribute != null)
            {
                value = new(ParseVector4(attribute.Value));
                return true;
            }
            return false;
        }
        private static bool TryColorFromAttribute(XAttribute? attribute, ref Color? value)
        {
            if (attribute != null)
            {
                value = new(ParseVector4(attribute.Value));
                return true;
            }
            return false;
        }
        private static bool TryRectFromAttribute(XAttribute? attribute, ref Rectangle value)
        {
            if (attribute != null)
            {
                value = ParseVector4(attribute.Value).ToRect();
                return true;
            }
            return false;
        }
        private static bool TryRectFromAttribute(XAttribute? attribute, ref Rectangle? value)
        {
            if (attribute != null)
            {
                value = ParseVector4(attribute.Value).ToRect();
                return true;
            }
            return false;
        }
    }
}
