﻿using ItemChanger;
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
        public override void OnBeforeLoad()
        {
            PlaceTransition("Deepnest_10", "right4", "Fungus2_34", "left1", 72, 138, new Vector2(2, 6), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default);
        }
        public override void OnWorldInit()
        {
            PlaceTransition("Deepnest_10", "right4", "Fungus2_34", "left1", 72, 138, new Vector2(2, 6), new Vector2(0, 0), GameManager.SceneLoadVisualizations.Default);
        }
    }
}
