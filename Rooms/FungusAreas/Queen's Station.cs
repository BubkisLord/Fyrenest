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

namespace Fyrenest.Rooms.FungalWastes
{
    internal class QueensStation : Room
    {
        public QueensStation() : base("Fungus2_01") { }
        public override void OnBeforeLoad()
        {
        }
        public override void OnWorldInit()
        {
            SetColor(Color.cyan);
            SetSaturation(100);
        }
    }
    internal class Fungus1_22 : Room
    {
        public Fungus1_22() : base("Fungus1_22") { }
        public override void OnBeforeLoad()
        {
            DestroyGO("Stag_Pole_Break");
        }
    }
    internal class Fungus1_21 : Room
    {
        public Fungus1_21() : base("Fungus1_21") { }
        public override void OnBeforeLoad()
        {
            PlaceGO(Prefabs.BREAKABLE_WALL.Object, 0.7f, 13, Quaternion.Euler(0, 0, -90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 0.6f, 13, Quaternion.Euler(0, 0, -90));
        }
    }
    internal class Cliffs_01 : Room
    {
        public Cliffs_01() : base("Cliffs_01") { }
        public override void OnBeforeLoad()
        {
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 113, 7, Quaternion.Euler(0, 0, -90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 113, 5, Quaternion.Euler(0, 0, -90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 113, 9, Quaternion.Euler(0, 0, -90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 113, 11, Quaternion.Euler(0, 0, -90));
            PlaceGO(Prefabs.LARGE_PLATFORM.Object, 113, 13, Quaternion.Euler(0, 0, -90));
            SetTransition("Cliffs_01", "right4", "Fungus1_28", "entrance");
        }
    }
    internal class Fungus1_16_alt : Room
    {
        public Fungus1_16_alt() : base("Fungus1_16_alt") { IsFlipped = true; }
    }
    internal class Fungus2_34 : Room
    {
        // Grassy room
        public Fungus2_34() : base("Fungus2_34") { }

        public override void OnBeforeLoad()
        {
            PlaceTransition("Fungus2_34", "left1", "Deepnest_10", "right4", 21, 7, new Vector2(2, 6), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default);
        }
        public override void OnWorldInit()
        {
            PlaceTransition("Fungus2_34", "left1", "Deepnest_10", "right4", 21, 7, new Vector2(2, 6), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default);
            SetTransition("Fungus2_34", "left1", "Deepnest_10", "right1");
            SetTransition("Fungus1_20_v02", "right1", "Fungus1_16_alt", "right1");
            SetTransition("Fungus1_21", "left1", "Fungus1_28", "left1");
            SetTransition("Fungus1_22", "left1", "Fungus1_04", "right1");
            
            // Insert the Moss boss room in between 2 queen's garden rooms
            SetTransition("Fungus3_08", "left1", "Fungus1_29", "right1");
            SetTransition("Deepnest_43", "right1", "Fungus1_29", "left1");

            // Skip the moss boss in greenpath
            SetTransition("Fungus1_11", "left1", "Fungus1_12", "right1");

            AbstractPlacement placement = Finder.GetLocation(LocationNames.Geo_Rock_Queens_Gardens_Below_Stag).Wrap();
            AbstractItem aitem = new WyrmForm();
            aitem.OnGive += OnGivenWyrmForm;
            aitem.UIDef = new MsgUIDef()
            {
                name = new BoxedString("Wings of Air"),
                shopDesc = new BoxedString("A strange looking piece of art. It resembles the birds in Greenpath."),
                sprite = new BoxedSprite(EmbeddedSprite.Get("WyrmForm.png")),
            };

            placement.Add(aitem);
            ItemChangerMod.AddPlacements(new AbstractPlacement[] { placement }, PlacementConflictResolution.MergeKeepingNew);
        }

        private void OnGivenWyrmForm(ReadOnlyGiveEventArgs obj)
        {
            WyrmForm.instance.SetObtained();
            WyrmForm.instance.Settings(Fyrenest.instance.Settings).Got = true;
        }
    }
    internal class Fungus1_16_Alt : Room
    {
        public Fungus1_16_Alt() : base("Fungus1_16_Alt") { IsFlipped = true; }
    }
}
