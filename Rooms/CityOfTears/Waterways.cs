using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fyrenest.Rooms.CityOfTears
{
    internal class City2_10b : Room
    {
        public City2_10b() : base("Ruins2_10b") { }

        public override void OnLoad()
        {
            /*PlaceGO(Prefabs.LARGE_PLATFORM.Object, 28, 138, Quaternion.Euler(0, 0, 90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 28, 140, Quaternion.Euler(0, 0, 90));*/

            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 5, 91);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 15, 93);
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 25, 91);
        }
    }
    internal class Waterways_03 : Room
    {
        public Waterways_03() : base("Waterways_03") { }

        public override void OnLoad()
        {
            // when seen tuk with defenders crest, tuk sells essence.
            if (!SlyDeal.instance.Equipped()) SetItem(LocationNames.Rancid_Egg_Tuk_Defenders_Crest, ItemNames.Whispering_Root_Crystal_Peak, false, 100, 0, 0, "Essence", "A yucky air-like substance which sometimes condenses into small orbs.");
            // when wearing sly deal, tuk decides to sell rancid eggs.
            else SetItem(LocationNames.Rancid_Egg_Tuk_Defenders_Crest, ItemNames.Rancid_Egg, true, alternateName:"Egg", alternateDesc:"Just take it! The stench coming off you is foul!");
        }
    }
}
