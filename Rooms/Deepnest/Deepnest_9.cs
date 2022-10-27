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

namespace Fyrenest.Rooms.Deepnest
{
    internal class Deepnest_9 : Room
    {
        public Deepnest_9() : base("Deepnest_09") { }
        public override void OnBeforeLoad()
        {
            SetColor(Color.black);
            SetSaturation(10);
            if (PlayerData.instance.royalCharmState != 3)
            {
                PlaceGO(Prefabs.LARGE_PLATFORM.Object, 0.5f, 5, Quaternion.Euler(0, 0, 270));
                PlaceGO(Prefabs.LARGE_PLATFORM.Object, 0.5f, 6, Quaternion.Euler(0, 0, 270));
                PlaceGO(Prefabs.LARGE_PLATFORM.Object, 0.5f, 7, Quaternion.Euler(0, 0, 270));
                PlaceGO(Prefabs.LARGE_PLATFORM.Object, 0.5f, 8, Quaternion.Euler(0, 0, 270));
            }
        }
        public override void OnWorldInit()
        {
            // Go to joni's blessing area.
            SetTransition("Deepnest_09", "left1", "Abyss_08", "right1");

            // Make it possible to get to Deepnest Stag station.
            SetItem(LocationNames.Geo_Rock_Abyss_3, ItemNames.Distant_Village_Stag);

            // Make deepnest stag go to void heart.
            SetItem(LocationNames.Jonis_Blessing, ItemNames.Void_Heart);

            // Get world sense after The Climb.
            SetItem(LocationNames.Void_Heart, ItemNames.World_Sense);

            // Get Joni's Blessing at hk room.
            SetItem(LocationNames.World_Sense, ItemNames.Jonis_Blessing);
        }
    }
}
