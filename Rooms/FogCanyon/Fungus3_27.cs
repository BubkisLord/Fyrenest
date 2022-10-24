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

namespace Fyrenest.Rooms.FogCanyon
{
    internal class Fungus3_01 : Room
    {
        public Fungus3_01() : base("Fungus3_01") { }
        public override void OnWorldInit()
        {
            // Take out the archives room
            SetTransition("Fungus3_01", "right1", "Fungus3_25", "left1");
            SetTransition("Fungus3_02", "right1", "Fungus3_27", "left1");
        }
    }
}
