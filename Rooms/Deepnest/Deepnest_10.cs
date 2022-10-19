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

namespace Fyrenest.Rooms.Deepnest
{
    internal class Deepnest_10 : Room
    {
        public Deepnest_10() : base("Deepnest_10") { }

        public override void OnWorldInit()
        {
            SetTransition("Deepnest_10", "right1", "Fungus2_01", "left2");
        }
    }
}
