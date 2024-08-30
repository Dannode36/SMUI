using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Characters;
using static System.Net.Mime.MediaTypeNames;

namespace SMUI.Elements
{
    public abstract class Container : Element
    {
        /*********
        ** Fields
        *********/
        private readonly IList<Element> ChildrenImpl = new List<Element>();

        /// <summary>Whether to update the <see cref="Children"/> when <see cref="Update"/> is called.</summary>
        protected bool UpdateChildren { get; set; } = true;

        public virtual bool Shadow { get; set; } = true;

        private Element? renderLast = null;
        public Element? RenderLast
        {
            get => renderLast;
            set
            {
                renderLast = value;
                if (Parent != null)
                {
                    if (value == null)
                    {
                        if (Parent.RenderLast == this)
                        {
                            Parent.RenderLast = null;
                        }
                        else
                        {
                            Parent.RenderLast = this;
                        }
                    }
                }
            }
        }

        public Element[] Children => ChildrenImpl.ToArray();

        public bool Selected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool ClickConsumed = false;
        public Element? ClickConsumer = null;

        public override bool Clickable { get; set; } = false;

        /*********
        ** Public methods
        *********/
        public void AddChild(Element element)
        {
            element.Parent?.RemoveChild(element);
            ChildrenImpl.Add(element);
            element.Parent = this;
        }

        public bool RemoveChild(Element element)
        {
            if (element.Parent != this)
            {
                return false;
            }
            element.Parent = null;
            return ChildrenImpl.Remove(element);
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);

            RenderLast = null;
            ClickConsumed = false;
            ClickConsumer = null;

            if (Enabled && UpdateChildren)
            {
                for (int i = ChildrenImpl.Count - (1); i >= 0; i--)
                {
                    ChildrenImpl[i].Update(isOffScreen);

                    if (ChildrenImpl[i].Hover && !string.IsNullOrEmpty(ChildrenImpl[i].Tooltip))
                    {
                        RenderLast = ChildrenImpl[i];
                    }
                }
            }

            if (Parent != null) //Update parent container if a nested container
            {
                Parent.ClickConsumed = ClickConsumed;
                Parent.ClickConsumer = ClickConsumer;
            }
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;
            for (int i = ChildrenImpl.Count - 1; i >= 0; i--)
            {
                if (ChildrenImpl[i] == RenderLast)
                    continue;
                ChildrenImpl[i].Draw(b);
            }
            RenderLast?.Draw(b);
        }
    }
}
