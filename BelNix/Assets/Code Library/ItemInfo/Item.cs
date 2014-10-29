using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemInfo
{
    abstract class Item
    {
        abstract public bool isSellable();

        // What properties or functions make up an item?
            // - Key Items: No value, not sellable, often used as triggers/keys or passports, 
            //      sometimes for game mechanics (such as in the case of a lantern)
            // - Normal items: Some value (potentially), sellable, sometimes used as triggers/passes
            //      - Equipment
            //          - probably want an IEquippable interface
            //              - why? Because what if, for example, you must equip a key item, such as a very
            //                  particular piece of a uniform or a plot-required sword or something, and it needs to
            //                  be usable, but not sellable
            //      - Consumables
            //          - IConsumable interface?
            //      - Crafting Materials
            //      - Misc items ("Ooh, look at this heavily attractive boulder, I want to bring it back to town.")
    }

    abstract class KeyItem : Item
    {
        // Key items can't be sold
        public override bool isSellable() { return false; }
    }
    abstract class StandardItem : Item
    {
        // Standard items can be sold
        public override bool isSellable() { return true; }
    }
}
