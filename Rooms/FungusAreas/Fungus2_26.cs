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
    internal class Fungus2_26 : Room
    {
        //Leg eater room
        public Fungus2_26() : base("Fungus2_26") { }
        public override void OnBeforeLoad()
        {
            PlaceTransition("Fungus2_26", "right1", "Fungus3_25", "left1", 53, 7, new Vector2(2.5f, 3), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default);
            SetTransition("Fungus3_25", "left1", "Fungus2_26", "right1");
        }
        public override void OnLoad()
        {
        }
        public override void OnWorldInit()
        {
            SetTransition("Fungus3_25", "right1", "Fungus3_47", "left1");
        }
    }
}
