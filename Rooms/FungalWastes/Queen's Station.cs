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
            SetTransition("Fungus2_01", "left3", "Fungus2_02", "right1");
        }
        public override void OnWorldInit()
        {
            SetColor(Color.cyan);
            SetSaturation(100);
        }
    }
    internal class Fungus2_34 : Room
    {
        public Fungus2_34() : base("Fungus2_34") { }

        public override void OnBeforeLoad()
        {
            PlaceTransition("Fungus2_34", "left1", "Deepnest_10", "right1", 21, 7, new Vector2(2, 6), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default);
            //CreateGateway("left1", 30, 16, new Vector2(10, 10), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default);
            //SetTransition("Fungus2_34", "left1", "Deepnest_10", "right1");
        }
    }
}
