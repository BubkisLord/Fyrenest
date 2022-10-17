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

namespace Fyrenest.Rooms.Crossroads
{
    internal class Crossroads1 : Room
    {
        public Crossroads1() : base("Crossroads_01") { }

        public override void OnBeforeLoad()
        {
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 37.2f, 6f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 37.2f, 7f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 37.2f, 8f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 38.2f, 6f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 38.2f, 7f);
            PlaceGO(Prefabs.BREAKABLE_FLOOR.Object, 38.2f, 8f);

            SetItem(LocationNames.Geo_Rock_Crossroads_Well, ItemNames.Charm_Notch);
        }
    }
}
