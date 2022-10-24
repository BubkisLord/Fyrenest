using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fyrenest.Rooms.CityOfTears
{
    internal class Ruins2_03 : Room
    {
        public Ruins2_03() : base("Ruins2_03") { }

        public override void OnBeforeLoad()
        {
            SetItem(LocationNames.Hallownest_Seal_City_Rafters, ItemNames.Geo_Rock_Outskirts420);

            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 47, 111);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 46, 111);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 45, 111);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 44, 111);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 43, 111);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 42, 111);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 41, 111);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 40, 111);
        }
    }
}
