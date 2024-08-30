using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMUI.Elements;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SMUI
{
    public class Layout
    {
        public RootElement root = new();
        public Dictionary<string, Element> uuidElements = new();
    }

    public static class LayoutHelper
    {
        public static IModHelper helper;
        public static IMonitor monitor;

        static private Dictionary<string, string> layoutNameToPath = new();
        public static List<Layout> Layouts = new();

        //possibly some crap code here
        private static string rootPath = string.Empty;

        public static void LoadLayout(string name)
        {
            XDocument layoutDoc = XDocument.Load(layoutNameToPath[name]);
            var tempRoot = Parse(layoutDoc);
            if(tempRoot != null)
            {
                Layouts.Add(tempRoot);
            }
            else
            {
                monitor.Log("Invalid XML layout", LogLevel.Error);
            }
        }

        public static Layout? Parse(XDocument xml)
        {
            RootElement root = new();
            if (xml.Root == null)
            {
                return null;
            }
            foreach (var element in xml.Root.Elements())
            {
                root.AddChild(ParseGenericElement(element));
            }
        }

        private static Element ParseGenericElement(XElement xElement)
        {
            Element element = ParseElementTag(xElement);

            var posAttr = xElement.Attribute("position");
            if (posAttr != null)
            {
                element.LocalPosition = ParseVector2(posAttr.Value);
            }

            var hoverSoundAttr = xElement.Attribute("hoverSound");
            if (hoverSoundAttr != null)
            {
                element.HoveredSound = hoverSoundAttr.Value;
            }

            var clickSoundAttr = xElement.Attribute("clickSound");
            if (clickSoundAttr != null)
            {
                element.ClickedSound = clickSoundAttr.Value;
            }

            var tooltipAttr = xElement.Attribute("tooltip");
            if (tooltipAttr != null)
            {
                element.Tooltip = tooltipAttr.Value;
            }

            var enabledAttr = xElement.Attribute("enabled");
            if (enabledAttr != null)
            {
                element.Enabled = bool.Parse(enabledAttr.Value);
            }

            return element;
        }

        private static Element ParseElementTag(XElement xElement)
        {
            switch (xElement.Name.LocalName)
            {
                case "Button":
                    var textureAttr = xElement.Attribute("texture");
                    if(textureAttr == null)
                    {
                        monitor.Log("Button Tag did not specify a texture");
                        return new Button();
                    }

                    var rectAttr = xElement.Attribute("rect");
                    if (rectAttr == null)
                    {
                        monitor.Log("Button Tag did not specify a rect");
                        return new Button();
                    }
                    Button button = new(GetBestTexture(textureAttr.Value), ParseVector4(rectAttr.Value).ToRect());

                    //OPTIONALS
                    var boxDrawAttr = xElement.Attribute("boxDraw");
                    if (boxDrawAttr != null)
                    {
                        button.BoxDraw = bool.Parse(boxDrawAttr.Value);
                    }

                    var idleTintAttr = xElement.Attribute("idleTint");
                    if (idleTintAttr != null)
                    {
                        button.IdleTint = new(ParseVector4(idleTintAttr.Value));
                    }

                    var hoverTintAttr = xElement.Attribute("hoverTint");
                    if (hoverTintAttr != null)
                    {
                        button.HoverTint = new(ParseVector4(hoverTintAttr.Value));
                    }

                    var sizeAttr = xElement.Attribute("size");
                    if (sizeAttr != null)
                    {
                        button.Size = ParseVector2(sizeAttr.Value);
                    }

                    return button;
                case "Checkbox":
                    break;
                case "Dropdown":
                    break;
                case "FloatBox":
                    break;
                case "Image":
                    break;
                case "IntBox":
                    break;
                case "ItemSlot":
                    break;
                case "ItemWithBorder":
                    break;
                case "Label":
                    break;
                case "Option":
                    break;
                case "Row":
                    break;
                case "Scrollbar":
                    break;
                case "Slider":
                    break;
                case "StaticContainer":
                    StaticContainer staticContainer = new();
                    foreach (var element in xElement.Elements())
                    {
                        staticContainer.AddChild(ParseElementTag(element));
                    }
                    return staticContainer;
                case "Table":
                    break;
                case "Textbox":
                    break;
                case "DateTimePicker":
                    break;
                case "DateTimePickerPopup":
                    break;
                default:
                    monitor.Log("Invalid element tag");
                    throw new();
            }
        }

        private static Texture2D? GetBestTexture(string name)
        {
            var game1Prop = (Texture2D?)typeof(Game1).GetProperty(name)?.GetValue(null);
            if (game1Prop != null)
            {
                return game1Prop;
            }

            var modTexture = helper.ModContent.Load<Texture2D>(rootPath + "/" + name);
            if (modTexture != null)
            {
                return modTexture;
            }

            monitor.Log($"No texture with the name \"{name}\" could be found", LogLevel.Error);
            return null;
        }

        private static Vector2 ParseVector2(string definition)
        {
            float[] values = Array.ConvertAll(definition.Split(','), s => float.Parse(s));

            if (values.Length < 2)
            {
                monitor.Log("Invalid rectangle attribute", LogLevel.Error);
                return Vector2.Zero;
            }

            return new(values[0], values[1]);
        }
        private static Vector3 ParseVector3(string definition)
        {
            float[] values = Array.ConvertAll(definition.Split(','), s => float.Parse(s));

            if (values.Length < 3)
            {
                monitor.Log("Invalid rectangle attribute", LogLevel.Error);
                return Vector3.Zero;
            }

            return new(values[0], values[1], values[2]);
        }
        private static Vector4 ParseVector4(string definition)
        {
            float[] values = Array.ConvertAll(definition.Split(','), s => float.Parse(s));

            if(values.Length < 4)
            {
                monitor.Log("Invalid rectangle attribute", LogLevel.Error);
                return Vector4.Zero;
            }

            return new(values[0], values[1], values[2], values[3]);
        }
        private static Rectangle ToRect(this Vector4 vector)
        {
            return new((int)vector.X, (int)vector.Y, (int)vector.Z, (int)vector.W);
        }
    }
}
