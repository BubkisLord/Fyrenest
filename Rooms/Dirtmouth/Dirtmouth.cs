using ItemChanger;
using ItemChanger.Components;
using ItemChanger.Internal;
using ItemChanger.Locations;
using ItemChanger.Locations.SpecialLocations;
using ItemChanger.Placements;
using ItemChanger.UIDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.Abyss
{
    internal class Dirtmouth : Room
    {
        public Dirtmouth() : base("Town") { }

        public override void OnBeforeLoad()
        {
            base.OnBeforeLoad();
        }
        public override void OnLoad()
        {
            AbstractPlacement placement = Finder.GetLocation(LocationNames.Iselda).Wrap();
            AbstractItem aitem = new SoulSlow();
            aitem.UIDef = new MsgUIDef()
            {
                name = new BoxedString("Wierd rock"),
                shopDesc = new BoxedString("I found this rock on the ground right outside my shop, it looks nice, so I am selling it at a value. It is worth it though, look at that art!"),
                sprite = new BoxedSprite(EmbeddedSprite.Get("SoulSlow.png")),
            };
                
            AbstractPlacement placement2 = Finder.GetLocation(LocationNames.Iselda).Wrap();
            AbstractItem aitem2 = new VoidSoul();
            aitem2.UIDef = new MsgUIDef()
            {
                name = new BoxedString("Strange Relic"),
                shopDesc = new BoxedString("Cornifer found this powerful and ancient relic deep down in Fyrenest."),
                sprite = new BoxedSprite(EmbeddedSprite.Get("VoidSoul.png")),
            };
            aitem.AddTag<CostTag>().Cost = new GeoCost(1200);
            aitem2.AddTag<CostTag>().Cost = new GeoCost(8000);
            placement.Add(aitem); 
            placement2.Add(aitem2);
            ItemChangerMod.AddPlacements(new AbstractPlacement[] { placement, placement2 }, PlacementConflictResolution.MergeKeepingNew);
        }
    }
}
