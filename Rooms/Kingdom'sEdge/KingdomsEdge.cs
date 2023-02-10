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

namespace Fyrenest.Rooms.KingdomsEdge
{
    internal class KingdomsEdge : Room
    {
        public KingdomsEdge() : base("Deepnest_East_01") { }
        public override void OnWorldInit()
        {
            AbstractPlacement placement = Finder.GetLocation(LocationNames.Kings_Brand).Wrap();
            AbstractItem aitem = new VoidSoul();
            aitem.UIDef = new MsgUIDef()
            {
                name = new BoxedString("VoidSoul"),
                sprite = new BoxedSprite(EmbeddedSprite.Get("VoidSoul.png")),
            };
            placement.Add(aitem);
            ItemChangerMod.AddPlacements(new AbstractPlacement[] { placement }, PlacementConflictResolution.MergeKeepingNew);
        }
    }
}
