using System;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.UIDefs;

namespace Fyrenest
{
    internal static class RandomizerCompatibility
    {
        public static void DefineItems()
        {

            ItemChanger.Items.CharmItem voidSoul = new()
            {
                charmNum = 0,
                name = "VoidSoul",
                UIDef = new SplitUIDef()
                {
                    sprite = new Sprite(),
                    shopDesc = new LanguageString("Shop Names", "VOIDSOUL_NAME"),
                    preview = new BoxedString("Void Soul"),
                    name = new BoxedString("VoidSoul")
                }
            };
            Finder.DefineCustomItem(voidSoul);
        }
    }
}