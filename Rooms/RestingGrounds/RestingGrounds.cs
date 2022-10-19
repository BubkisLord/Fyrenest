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

namespace Fyrenest.Rooms.RestingGrounds
{
    internal class RestingGrounds_09 : Room
    {
        public RestingGrounds_09() : base("RestingGrounds_09") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }

        public override void OnWorldInit()
        {
            SetTransition("RestingGrounds_05", "right1", "GG_Workshop", "left1");
        }
    }
    internal class RestingGrounds_10 : Room
    {
        public RestingGrounds_10() : base("RestingGrounds_10") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
        public override void OnWorldInit()
        {
            // Go to the grassy room in Queen's Station instead of normal Resting Grounds stag station.
            SetTransition("RestingGrounds_05", "right2", "Fungus3_35", "right1");

            // Switch Slug room and Sheo's Hut room (not the inside room).
            SetTransition("Fungus1_26", "left1", "Fungus1_15", "right1");
            SetTransition("Fungus1_09", "left1", "Fungus1_Slug", "right1");

            // Make nailmasters give essence instead:
            SetItem(LocationNames.Cyclone_Slash, ItemNames.Boss_Essence_Gorb, geoCost: 600, alternateName: "Spiritual Energy I");
            SetItem(LocationNames.Dash_Slash, ItemNames.Boss_Essence_Failed_Champion, geoCost: 900, alternateName: "Spiritual Energy II");
            SetItem(LocationNames.Great_Slash, ItemNames.Boss_Essence_Soul_Tyrant, geoCost: 1200, alternateName: "Spiritual Energy III");
            SetItem(LocationNames.Nailmasters_Glory, ItemNames.Boss_Essence_Markoth, alternateName: "Spiritual Energy IV");

            // Make DreamShield, Soul Eater, and some stuff in the catacombs give nailmaster stuff
            SetItem(LocationNames.Dreamshield, ItemNames.Cyclone_Slash);
            SetItem(LocationNames.Soul_Eater, ItemNames.Great_Slash);
            SetItem(LocationNames.Geo_Rock_Resting_Grounds_Catacombs_Left, ItemNames.Dash_Slash);
            SetItem(LocationNames.Hallownest_Seal_Resting_Grounds_Catacombs, ItemNames.Nailmasters_Glory);
            
            //Place Soul Eater in Salubra's house so it is still obtainable.
            SetItem(LocationNames.Salubra, ItemNames.Soul_Eater, true, 2000, alternateName: "Unknown Charm", alternateDesc: "I found this charm floating outside my house. I brought it in, but I haven't worn it. I'm sure a brave knight such as you would dare to wear this possibly damgerous item!\n\nI also made it expensive, because it might be super special! I may have great prices, but not that great!");

            // Make it possible to get to the now-hidden spirit glade.
            SetItem(LocationNames.Resting_Grounds_Map, ItemNames.Resting_Grounds_Stag, geoCost: 170);

            // Map all rooms upon loading with Fyrenest enabled.
            PlayerData.instance.mapAllRooms = true;

            // Make a stand-alone area with the hidden spirit glade.
            SetTransition("RestingGrounds_08", "left1", "RestingGrounds_09", "right1");

        }
    }
    internal class DreamShieldRoom: Room
    {
        public DreamShieldRoom() : base("RestingGrounds_17") { }

        public override void OnBeforeLoad()
        {
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 50, 5, Quaternion.Euler(0, 0, 45));
            if (PlayerData.instance.dreamReward9)
            {
                PlaceGO(Prefabs.PANTHEON_V.Object, 15.5f, 7);
                PlaceGO(Prefabs.TOP_TRANSITION.Object, 15.5f, 12);
                SetTransition("RestingGrounds_17", "top1", "RestingGrounds_17", "right1", true);
            }
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }

        public override void OnWorldInit()
        {
            PlaceGO(Prefabs.LEFT_TRANSITION.Object, 15, 17);
            SetTransition("RestingGrounds_17", "left1", "Deepnest_10", "right1", true);
        }
    }
    internal class Fungus3_35 : Room
    {
        public Fungus3_35() : base("Fungus3_35") { IsFlipped = true; }
    }
    internal class GG_Workshop : Room
    {
        public GG_Workshop() : base("GG_Workshop") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetEnvironment(0);
            SetColor(Color.gray);
        }
    }
    internal class RestingGrounds_01 : Room
    {
        public RestingGrounds_01() : base("RestingGrounds_01") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_02 : Room
    {
        public RestingGrounds_02() : base("RestingGrounds_02") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_03 : Room
    {
        public RestingGrounds_03() : base("RestingGrounds_03") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_04 : Room
    {
        public RestingGrounds_04() : base("RestingGrounds_04") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class Crossroads_46b : Room
    {
        public Crossroads_46b() : base("Crossroads_46b") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class Crossroads_50 : Room
    {
        public Crossroads_50() : base("Crossroads_50") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class Ruins2_10 : Room
    {
        public Ruins2_10() : base("Ruins2_10") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_05 : Room
    {
        public RestingGrounds_05() : base("RestingGrounds_05") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
            if (PlayerData.instance.dreamReward9)
            {
                SetTransition("RestingGrounds_07", "right1", "RestingGrounds_17", "right1"); 
            }
        }
    }
    internal class RestingGrounds_06 : Room
    {
        public RestingGrounds_06() : base("RestingGrounds_06") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_07 : Room
    {
        public RestingGrounds_07() : base("RestingGrounds_07") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_08 : Room
    {
        public RestingGrounds_08() : base("RestingGrounds_08") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_11 : Room
    {
        public RestingGrounds_11() : base("RestingGrounds_11") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_12 : Room
    {
        public RestingGrounds_12() : base("RestingGrounds_12") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_13 : Room
    {
        public RestingGrounds_13() : base("RestingGrounds_13") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_14 : Room
    {
        public RestingGrounds_14() : base("RestingGrounds_14") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_15 : Room
    {
        public RestingGrounds_15() : base("RestingGrounds_15") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
    internal class RestingGrounds_16 : Room
    {
        public RestingGrounds_16() : base("RestingGrounds_16") { }

        public override void OnBeforeLoad()
        {
            SetSaturation(0);
            SetColor(Color.gray);
            SetEnvironment(0);
        }
    }
}
