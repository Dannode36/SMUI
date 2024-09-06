using StardewValley;

namespace SMUI.Elements
{
    public class Root : Container
    {
        /*********
        ** Accessors
        *********/
        public bool Obscured { get; set; } = false;

        public override int Width => Game1.viewport.Width;
        public override int Height => Game1.viewport.Height;
        public override bool Clickable { get; set; } = false;

        /*********
        ** Public methods
        *********/
        public Root()
        {
            ModEntry.AddToUpdateList(this);
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen || this.Obscured);
            if (Dropdown.ActiveDropdown?.GetRoot() != this)
            {
                Dropdown.ActiveDropdown = null;
            }
            if ( Dropdown.SinceDropdownWasActive > 0 )
            {
                Dropdown.SinceDropdownWasActive--;
            }
        }

        /// <inheritdoc />
        internal override Root GetRootImpl()
        {
            return this;
        }
    }
}
