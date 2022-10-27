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

namespace Fyrenest.Rooms.CrystalPeak
{
    internal class Mines_37 : Room
    {
        public Mines_37() : base("Mines_37") { }

        public override void OnWorldInit()
        {
            // Make this go somewhere other than the deepnest stag station
            PlaceTransition("Mines_37", "left1", "Crossroads_30", "left1", 2.25f, 24, new Vector2(1.5f, 2.5f), new Vector2(2, 0), GameManager.SceneLoadVisualizations.Default);
        }
    }
    internal class Crossroads_30 : Room
    {
        // Flip target room
        public Crossroads_30() : base("Crossroads_30") { IsFlipped = true; }
    }
}
